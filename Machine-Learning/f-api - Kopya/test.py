import requests

url = 'http://localhost:5000/predict'

data = {
    "tarih": "2025-06-15",  # Yaz günü
    "saat": 19,             # Öğle vakti
    "sicaklik": 32.0,       # Sıcak
    "lag_24h": 35000.0      # Dünkü tüketim
}

response = requests.post(url, json=data)
print(response.json())