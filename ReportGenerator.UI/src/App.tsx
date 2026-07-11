import { useState, useRef } from 'react'
import axios from 'axios'

function App() {
  const [reportName, setReportName] = useState('')
  const [status, setStatus] = useState<string>('Brak zlecenia')
  const [reportId, setReportId] = useState<string | null>(null)
  
  // Referencja do przechowywania interwału, żebyśmy mogli go zatrzymać
  const intervalRef = useRef<number | null>(null);

  const API_URL = 'https://localhost:7214/api/reports';

  const checkStatus = async (id: string) => {
    try {
      const response = await axios.get(`${API_URL}/${id}`);
      const currentStatus = response.data.status;
      
      setStatus(`Sprawdzanie... Obecny status w bazie: ${currentStatus}`);

      if (currentStatus === 'Completed') {
        setStatus('Gotowe! Twój plik PDF czeka na dysku serwera.');
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
      setStatus('Wysyłanie zlecenia...');
      
      const response = await axios.post(API_URL, {
        name: reportName
      });

      if (response.status === 202) {
        // Zakładamy, że Twój POST zwraca po prostu Guid w postaci tekstu lub w obiekcie
        const newReportId = response.data.reportId;
        setReportId(newReportId);
        setStatus('Zlecenie przyjęte! (Status 202 - oczekiwanie na Workera...)');

        // Uruchamiamy odpytywanie co 2 sekundy (2000 ms)
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

      <div style={{ padding: '20px', backgroundColor: '#e8f4f8', borderRadius: '8px' }}>
        <strong>Status na żywo: </strong> {status}
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