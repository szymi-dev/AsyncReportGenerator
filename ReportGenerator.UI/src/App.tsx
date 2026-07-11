import { useState, useRef } from 'react'
import axios from 'axios'

function App() {
  const [reportName, setReportName] = useState('')
  const [status, setStatus] = useState<string>('Brak zlecenia')
  const [reportId, setReportId] = useState<string | null>(null)
  
  const [fileUrl, setFileUrl] = useState<string | null>(null)
  
  const intervalRef = useRef<number | null>(null);

  const API_URL = 'https://localhost:7214/api/reports';

  const checkStatus = async (id: string) => {
    try {
      const response = await axios.get(`${API_URL}/${id}`);
      const currentStatus = response.data.status;
      
      setStatus(`Sprawdzanie... Obecny status w bazie: ${currentStatus}`);

      if (currentStatus === 'Completed') {
        setStatus('Gotowe! Twój plik PDF czeka na dysku serwera.');
        
        if (response.data.fileUrl) {
            setFileUrl(response.data.fileUrl);
        }

        if (intervalRef.current) {
            clearInterval(intervalRef.current);
        }
      }
    } catch (error) {
      console.error(error);
    }
  };

  const handleGenerateClick = async () => {
    if (!reportName) {
        alert("Podaj nazwę raportu!");
        return;
    }

    try {
      setFileUrl(null);
      setStatus('Wysyłanie zlecenia...');
      
      const response = await axios.post(API_URL, {
        name: reportName
      });

      if (response.status === 202) {
        console.log("Otrzymano z API:", response.data);

        const newReportId = response.data.reportId || response.data.ReportId; 
        
        if (!newReportId) {
            setStatus('Błąd: Backend nie zwrócił poprawnego ID.');
            return;
        }

        setReportId(newReportId);
        setStatus('Zlecenie przyjęte! (Status 202 - oczekiwanie na Workera...)');

        intervalRef.current = window.setInterval(() => {
            checkStatus(newReportId);
        }, 2000);
      }
    } catch (error) {
      console.error(error);
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