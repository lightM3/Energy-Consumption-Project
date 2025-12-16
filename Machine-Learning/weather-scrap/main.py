import time
import os
import pandas as pd
from datetime import date, timedelta
from playwright.sync_api import sync_playwright

# ==============================================================================
# 1. AYARLAR
# ==============================================================================
FINAL_CSV_FILE = "Hava_Durumu_Verileri.csv"
USER_DATA_DIR = "wunderground_chrome_profile"

# Şehirler ve Kodları
CITIES = {
    'Istanbul': 'bakirkoy/LTBA',
    'Ankara':   'cubuk/LTAC',
    'Izmir':    'gaziemir/LTBJ',
    'Erzurum':  'yakutiye/LTCE'
}

# Tarih Aralığı
DAYS_TO_SCRAPE = 500
start_date = date.today() - timedelta(days=DAYS_TO_SCRAPE)
end_date = date.today() - timedelta(days=1)

# ==============================================================================
# 2. YARDIMCI FONKSİYONLAR
# ==============================================================================
def clean_temp(temp_str):
    """Sıcaklık değerini temizler."""
    try:
        return float(temp_str.replace("°F", "").strip())
    except:
        return None

# ==============================================================================
# 3. ANA SCRİPT
# ==============================================================================
def run():
    print("🚀 Akıllı Hava Durumu Botu Başlatılıyor...")
    print(f"📅 Hedef: {start_date.strftime('%d.%m.%Y')} tarihinden başlayarak veri toplamak.")
    
    # --- KONTROL: Mevcut verileri oku ---
    existing_dates = set()
    if os.path.exists(FINAL_CSV_FILE):
        try:
            df_exist = pd.read_csv(FINAL_CSV_FILE)
            if "DateTime" in df_exist.columns:
                existing_dates = set(pd.to_datetime(df_exist["DateTime"]).dt.date)
                print(f"📂 Mevcut dosyada {len(existing_dates)} günlük veri bulundu.")
        except: pass

    with sync_playwright() as p:
        # Eski ayarların (Hata vermemesi için locale ekledim sadece)
        browser = p.chromium.launch_persistent_context(
            user_data_dir=USER_DATA_DIR,
            headless=False,
            viewport={"width": 1280, "height": 900},
            locale="en-US" # Saat formatı bozulmasın diye bu şart
        )
        page = browser.pages[0] if browser.pages else browser.new_page()
        
        print("\n" + "="*60)
        print("🛑 BEKLEME MODU")
        print("Sayfa açılınca her şey yolundaysa buraya gelip ENTER'a basın.")
        print("="*60 + "\n")
        
        # Test sayfası (Buraya timeout ekledim ki hata verip kapanmasın)
        try:
            page.goto(
                f"https://www.wunderground.com/history/daily/tr/cubuk/LTAC/date/{start_date.strftime('%Y-%m-%d')}", 
                wait_until="domcontentloaded", 
                timeout=60000
            )
        except:
            print("⚠️ İlk sayfa yavaş açıldı ama devam ediliyor...")

        input("👉 Hazır olduğunuzda ENTER tuşuna basın...")

        all_weather_data = []
        current_date = start_date

        try:  # --- ACİL DURDURMA BLOGU ---
            
            while current_date <= end_date:
                date_str_display = current_date.strftime("%d.%m.%Y")
                date_str_url = current_date.strftime("%Y-%m-%d")

                if current_date in existing_dates:
                    print(f"⏭️  {date_str_display} zaten var, atlanıyor.")
                    current_date += timedelta(days=1)
                    continue

                print(f"\n📆 TARİH İŞLENİYOR: {date_str_display}")
                print("-" * 40)

                for city_name, city_code in CITIES.items():
                    url = f"https://www.wunderground.com/history/daily/tr/{city_code}/date/{date_str_url}"
                    print(f"  📍 {city_name} verisi çekiliyor...", end="")

                    try:
                        # wait_until="domcontentloaded" -> Bu çok önemli, sayfanın tam yüklenmesini beklemez, hızlı geçer.
                        page.goto(url, wait_until="domcontentloaded", timeout=45000)
                        
                        try:
                            page.wait_for_selector("table.mat-table", timeout=6000)
                        except:
                            print(" ⚠️ Tablo yok.")
                            continue

                        rows = page.locator("table.mat-table tbody tr").all()
                        collected = 0
                        
                        for row in rows:
                            try:
                                cells = row.locator("td").all()
                                if len(cells) < 2: continue # Boş satır koruması

                                time_val = cells[0].inner_text().strip()
                                temp_val = cells[1].inner_text().strip()
                                
                                if time_val and temp_val:
                                    all_weather_data.append({
                                        "Sehir": city_name,
                                        "Tarih": date_str_display,
                                        "Saat_Raw": time_val,
                                        "Sicaklik_F": clean_temp(temp_val)
                                    })
                                    collected += 1
                            except: continue
                        
                        print(f" ✅ {collected} kayıt.")
                        
                    except Exception as e:
                        print(f" ❌ Hata: {e}")
                    
                    time.sleep(1)

                current_date += timedelta(days=1)
                
        except KeyboardInterrupt:
            print("\n🛑 KULLANICI TARAFINDAN DURDURULDU (Ctrl+C)")

        # ======================================================================
        # 4. VERİ İŞLEME VE KAYDETME (DÜZELTİLEN KISIM BURASI)
        # ======================================================================
        
        if all_weather_data:
            print("💾 Veriler işleniyor ve EKSİKLER DOLDURULUYOR...")
            df = pd.DataFrame(all_weather_data)
            
            # Tarih/Saat Formatla
            df["DateTime_Str"] = df["Tarih"] + " " + df["Saat_Raw"]
            # Hatalı tarih formatlarını yok say (coerce)
            df["DateTime"] = pd.to_datetime(df["DateTime_Str"], format="%d.%m.%Y %I:%M %p", errors='coerce')
            df = df.dropna(subset=["DateTime"])

            # Sıcaklık Dönüşümü (F -> C)
            df["Sicaklik_C"] = (df["Sicaklik_F"] - 32) * 5/9
            
            # --- EKSİK VERİ DOLDURMA (Interpolation) ---
            df.set_index("DateTime", inplace=True)
            
            # 1. Önce saatlik olarak grupla (Resample zaten eksik saatleri NaN olarak yaratır)
            df_hourly = df.groupby("Sehir")["Sicaklik_C"].resample("h").mean()
            
            # 2. Şimdi NaN olan o eksik saatleri doldur (Linear Interpolation)
            # limit=24: Eğer 24 saatten fazla veri yoksa orayı doldurma, salla olur çünkü.
            df_hourly_filled = df_hourly.interpolate(method='linear', limit=24).reset_index()
            
            # Pivot İşlemi (Tabloyu genişletme)
            df_pivot = df_hourly_filled.pivot(index="DateTime", columns="Sehir", values="Sicaklik_C").reset_index()
            
            # Sütun İsimleri ve Yuvarlama
            df_pivot.columns.name = None
            new_cols = ["DateTime"]
            for col in df_pivot.columns:
                if col != "DateTime":
                    new_cols.append(f"{col}_Sicaklik")
            df_pivot.columns = new_cols
            df_pivot = df_pivot.round(1)

            # Dosyaya Ekleme (Append Mantığı)
            if os.path.exists(FINAL_CSV_FILE):
                try:
                    df_old = pd.read_csv(FINAL_CSV_FILE)
                    df_old["DateTime"] = pd.to_datetime(df_old["DateTime"])
                    
                    df_final = pd.concat([df_old, df_pivot])
                    df_final = df_final.drop_duplicates(subset="DateTime", keep="last")
                except:
                    df_final = df_pivot
            else:
                df_final = df_pivot

            df_final = df_final.sort_values("DateTime")
            df_final.to_csv(FINAL_CSV_FILE, index=False)
            print(f"🎉 DOSYA GÜNCELLENDİ: {FINAL_CSV_FILE}")
            print(f"Toplam Kayıt: {len(df_final)}")
        else:
            print("⚠️ Yeni kaydedilecek veri yok.")
            
        browser.close()

if __name__ == "__main__":
    run()