from flask import Flask, request, jsonify
import joblib
import pandas as pd
import numpy as np
import os

# Flask uygulamasını başlat
app = Flask(__name__)

# --- MODEL YÜKLEME ---
# Dinamik dosya yolu: Kod nerede çalışırsa çalışsın, pkl dosyasını kendi yanından bulur.
MODEL_FILE = 'elektrik_tuketim_modeli.pkl'
MODEL_PATH = os.path.join(os.path.dirname(__file__), MODEL_FILE)

try:
    if os.path.exists(MODEL_PATH):
        model = joblib.load(MODEL_PATH)
        print(f"✅ Model başarıyla yüklendi: {MODEL_PATH}")
    else:
        print(f"❌ HATA: Model dosyası bulunamadı! Beklenen yol: {MODEL_PATH}")
        model = None
except Exception as e:
    print(f"❌ HATA: Model yüklenirken bir sorun oluştu.\n{e}")
    model = None

# --- SAĞLIK KONTROLÜ (Health Check) ---
@app.route('/', methods=['GET'])
def home():
    status = "AKTİF" if model else "MODEL YÜKLENEMEDİ"
    return jsonify({
        "servis": "Elektrik Tüketim Tahmini API",
        "durum": status,
        "kullanim": "POST isteği ile /predict endpoint'ini kullanın."
    })

# --- TAHMİN ENDPOINT'İ ---
@app.route('/predict', methods=['POST'])
def predict():
    # Model yüklü değilse işlem yapma
    if not model:
        return jsonify({'status': 'error', 'mesaj': 'Model sunucuda yüklü değil.'}), 500

    try:
        # 1. Gelen JSON verisini al
        data = request.get_json()
        
        if not data:
             return jsonify({'status': 'error', 'mesaj': 'JSON verisi gönderilmedi.'}), 400

        # Gelen veriyi değişkenlere ata
        tarih_str = data.get('tarih')       # Örn: "2025-11-25"
        saat = data.get('saat')             # Örn: 14
        sicaklik = data.get('sicaklik')     # Ağırlıklı Sıcaklık
        lag_24h = data.get('lag_24h')       # Bir gün önceki tüketim
        
        # Eksik veri kontrolü
        if None in [tarih_str, saat, sicaklik, lag_24h]:
            return jsonify({'status': 'error', 'mesaj': 'Eksik parametreler var (tarih, saat, sicaklik, lag_24h).'}), 400

        # Tip dönüşümleri
        saat = int(saat)
        sicaklik = float(sicaklik)
        lag_24h = float(lag_24h)

        # 2. Özellik Mühendisliği (Modelin istediği formata çevir)
        tarih_obj = pd.to_datetime(tarih_str)
        
        ay = tarih_obj.month
        haftanin_gunu = tarih_obj.dayofweek # 0=Pzt, 6=Paz
        
        # Hafta Sonu mu? (Cumartesi=5, Pazar=6)
        hafta_sonu = 1 if haftanin_gunu >= 5 else 0
        
        # Mesai Saati mi? (08:00 - 18:00 arası)
        mesai_saati = 1 if 8 <= saat <= 18 else 0
        
        # 3. DataFrame Oluştur (Model eğitimiyle aynı sütun sırası ŞART!)
        input_df = pd.DataFrame([[
            sicaklik,       # Weighted_Avg_Temp
            saat,           # Saat
            ay,             # Ay
            haftanin_gunu,  # Haftanin_Gunu
            hafta_sonu,     # Hafta_Sonu
            mesai_saati,    # Mesai_Saati
            lag_24h         # Lag_Tuketim_24h
        ]], columns=['Weighted_Avg_Temp', 'Saat', 'Ay', 'Haftanin_Gunu', 'Hafta_Sonu', 'Mesai_Saati', 'Lag_Tuketim_24h'])
        
        # 4. Tahmin Yap
        prediction = model.predict(input_df)
        
        # Sonucu yuvarla ve JSON olarak döndür
        sonuc = round(float(prediction[0]), 2)
        
        return jsonify({
            'status': 'success',
            'tarih': tarih_str,
            'saat': saat,
            'tahmin_mwh': sonuc,
            'mesaj': 'Tahmin başarıyla üretildi.'
        })

    except Exception as e:
        return jsonify({'status': 'error', 'hata_detayi': str(e)}), 400

# Uygulamayı çalıştır
if __name__ == '__main__':
    # Port 5000'de çalışacak
    print("🚀 ML Servisi Başlatılıyor...")
    app.run(debug=True, port=5000)