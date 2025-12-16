from flask import Flask, request, jsonify
import pickle
import pandas as pd
import numpy as np

app = Flask(__name__)

# Modeli Yükle
model_path = 'elektrik_tuketim_modeli.pkl'
try:
    with open(model_path, 'rb') as f:
        model = pickle.load(f)
    print(f"✅ Model başarıyla yüklendi: {model_path}")
except Exception as e:
    print(f"❌ HATA: Model yüklenemedi! {str(e)}")
    model = None

# Yardımcı Fonksiyon: 0 gelse bile değeri al
def get_val(data, keys):
    for k in keys:
        if k in data:
            return data[k]
    return None

@app.route('/predict', methods=['POST'])
def predict():
    try:
        data = request.get_json()
        print("\n--- C# TARAFINDAN GELEN VERİ ---")
        print(data) 

        # Verileri çek
        tarih_str = get_val(data, ['Tarih', 'tarih'])
        saat = float(get_val(data, ['Saat', 'saat']))
        sicaklik = float(get_val(data, ['Sicaklik', 'sicaklik']))
        lag = float(get_val(data, ['Lag_24', 'lag_24', 'lag_24h']))

        # --- FEATURE ENGINEERING (EKSİK VERİLERİ ÜRETME) ---
        # Tarih stringini (2025-10-21) tarih formatına çevir
        dt = pd.to_datetime(tarih_str)

        # 1. Ay (1-12)
        ay = dt.month

        # 2. Haftanın Günü (0=Pazartesi, 6=Pazar)
        haftanin_gunu = dt.dayofweek

        # 3. Hafta Sonu mu? (Cumartesi(5) veya Pazar(6) ise 1, değilse 0)
        hafta_sonu = 1 if haftanin_gunu >= 5 else 0

        # 4. Mesai Saati mi? (Sabah 8 - Akşam 18 arası ve Hafta içi ise 1)
        mesai_saati = 1 if (8 <= saat <= 18) and (hafta_sonu == 0) else 0

        # --- DATAFRAME OLUŞTURMA (MODELİN İSTEDİĞİ SIRAYLA) ---
        # Beklenen: ['Weighted_Avg_Temp', 'Saat', 'Ay', 'Haftanin_Gunu', 'Hafta_Sonu', 'Mesai_Saati', 'Lag_Tuketim_24h']
        
        features = pd.DataFrame({
            'Weighted_Avg_Temp': [sicaklik],  # Sicaklik -> Weighted_Avg_Temp
            'Saat': [saat],
            'Ay': [ay],
            'Haftanin_Gunu': [haftanin_gunu],
            'Hafta_Sonu': [hafta_sonu],
            'Mesai_Saati': [mesai_saati],
            'Lag_Tuketim_24h': [lag]          # Lag_24 -> Lag_Tuketim_24h
        })

        print("📊 Model İçin Hazırlanan Tablo (Son Hali):")
        print(features)

        # Tahmin Yap
        if model:
            prediction = model.predict(features)[0]
            print(f"✅ Tahmin Sonucu: {prediction}")
            return jsonify({
                'success': True,
                'tahmin': float(prediction),
                'prediction': float(prediction)
            })
        else:
            return jsonify({'success': False, 'mesaj': 'Model yüklü değil'})

    except Exception as e:
        print(f"❌ PYTHON HATASI: {str(e)}")
        # Hatayı net görelim
        import traceback
        traceback.print_exc()
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(port=5000, debug=True)