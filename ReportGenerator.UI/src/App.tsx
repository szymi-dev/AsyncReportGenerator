import { useState, useRef, useEffect } from 'react'
import axios from 'axios'
import { HubConnectionBuilder } from '@microsoft/signalr'

function App() {
  const [reportName, setReportName] = useState('')
  const [status, setStatus] = useState<string>('Łączenie z serwerem...')
  const [reportId, setReportId] = useState<string | null>(null)
  const [fileUrl, setFileUrl] = useState<string | null>(null)
  const activeReportIdRef = useRef<string | null>(null);

  const API_URL = 'https://localhost:7214/api/reports';
  const HUB_URL = 'https://localhost:7214/reportHub'; 

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect()
      .build();

    connection.start()
      .then(() => {
          setStatus('Gotowy do pracy (Połączono z serwerem w czasie rzeczywistym)');
      })
      .catch(err => console.error('Błąd SignalR: ', err));

    connection.on("ReportCompleted", (completedId: string, url: string) => {
      if (activeReportIdRef.current === completedId) {
        setStatus('Gotowe! Serwer wygenerował Twój raport.');
        setFileUrl(url);
      }
    });

    return () => {
      connection.stop();
    };
  }, []);

  const handleGenerateClick = async () => {
    if (!reportName) {
        alert("Podaj nazwę raportu!");
        return;
    }

    try {
      setFileUrl(null);
      setReportId(null);
      activeReportIdRef.current = null;
      setStatus('Wysyłanie zlecenia...');
      
      const response = await axios.post(API_URL, {
        name: reportName
      });

      if (response.status === 202) {
        const newReportId = response.data.reportId || response.data.ReportId; 
        
        if (!newReportId) {
            setStatus('Błąd: Backend nie zwrócił poprawnego ID.');
            return;
        }

        setReportId(newReportId);
        activeReportIdRef.current = newReportId; 
        setStatus('Zlecenie przyjęte! (Czekam na sygnał od Workera...)');
      }
    } catch (error) {
      setStatus('Wystąpił błąd podczas komunikacji z API.');
    }
  }

  return (
    <div style={{ padding: '40px', fontFamily: 'sans-serif' }}>
      <h1>Menedżer Raportów - Pan Jan</h1>
      
      <div style={{ marginBottom: '20px' }}>
        <input 
          type="text" 
          placeholder="Wpisz nazwę raportu..." 
          value={reportName}
          onChange={(e) => setReportName(e.target.value)}
          style={{ padding: '10px', width: '300px', marginRight: '10px' }}
        />
        <button onClick={handleGenerateClick} style={{ padding: '10px 20px', cursor: 'pointer' }}>
          Generuj Raport
        </button>
      </div>

      <div style={{ padding: '20px', backgroundColor: '#e8f4f8', borderRadius: '8px', display: 'flex', flexDirection: 'column', gap: '15px' }}>
        <div>
            <strong>Status na żywo: </strong> {status}
        </div>
        
        {fileUrl && (
            <a 
                href={fileUrl} 
                target="_blank" 
                rel="noopener noreferrer"
                style={{
                    padding: '10px 20px',
                    backgroundColor: '#4CAF50',
                    color: 'white',
                    textDecoration: 'none',
                    textAlign: 'center',
                    borderRadius: '5px',
                    fontWeight: 'bold',
                    width: 'fit-content'
                }}
            >
                ⬇️ Pobierz plik PDF
            </a>
        )}
      </div>
      
      {reportId && (
          <div style={{ marginTop: '10px', fontSize: '12px', color: 'gray' }}>
              Śledzone ID: {reportId}
          </div>
      )}
    </div>
  )
}

export default App