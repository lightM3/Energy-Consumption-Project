import os
import time
import pandas as pd
from datetime import date, timedelta
from playwright.sync_api import sync_playwright

# ==============================================================================
# 1. AYARLAR
# ==============================================================================
FINAL_CSV_FILE = "EPIAS_Gercek_Tuketim_Verileri.csv"
USER_DATA_DIR = "epias_oturum_profili"
URL = "https://seffaflik.epias.com.tr/electricity/electricity-consumption/ex-post-consumption/real-time-consumption"

# Kaç günlük veri kontrol edilecek?
DAYS_TO_SCRAPE = 184
start_date = date.today() - timedelta(days=DAYS_TO_SCRAPE)
end_date = date.today() - timedelta(days=1)

# ==============================================================================
# 2. ANA SCRİPT
# ==============================================================================

def run():
    print("🚀 Playwright EPİAŞ Botu Başlatılıyor...")

    # --- KONTROL MEKANİZMASI: Mevcut verileri oku ---
    existing_dates = set()
    if os.path.exists(FINAL_CSV_FILE):
        try:
            df_exist = pd.read_csv(FINAL_CSV_FILE)
            # DateTime sütununu tarih objesine çevirip sadece gün kısmını alıyoruz
            if "DateTime" in df_exist.columns:
                existing_dates = set(pd.to_datetime(df_exist["DateTime"]).dt.date)
                print(f"📂 Mevcut dosyada {len(existing_dates)} farklı günün verisi bulundu.")
        except Exception as e:
            print(f"⚠️ Mevcut dosya okunurken hata: {e}")

    with sync_playwright() as p:
        browser = p.chromium.launch_persistent_context(
            user_data_dir=USER_DATA_DIR,
            headless=False,
            viewport={"width": 1366, "height": 768},
            args=["--start-maximized"]
        )
        
        page = browser.pages[0] if browser.pages else browser.new_page()
        
        print(f"🌍 Siteye gidiliyor: {URL}")
        page.goto(URL, wait_until="domcontentloaded")
        
        # --- BEKLEME MODU ---
        print("\n" + "="*60)
        print("🛑 BEKLEME MODU")
        print("Giriş yapın ve 'Gerçek Zamanlı Tüketim' sayfasında olduğunuzdan emin olun.")
        print("="*60 + "\n")
        input("👉 Hazır olduğunuzda ENTER tuşuna basın...")
        
        print("✅ Veri toplama işlemi başlıyor...")
        
        new_data = []
        current_date = start_date

        while current_date <= end_date:
            date_str = current_date.strftime("%d.%m.%Y")
            
            # --- AKILLI KONTROL: Bu tarih zaten var mı? ---
            if current_date in existing_dates:
                print(f"⏭️  {date_str} zaten dosyada var, ATLANIYOR.")
                current_date += timedelta(days=1)
                continue
            # ----------------------------------------------

            print(f"📅 İşleniyor: {date_str}")
            
            try:
                # 1. Başlangıç Tarihi
                page.locator("input[name='startDate']").click()
                page.locator("input[name='startDate']").press("Control+a")
                page.locator("input[name='startDate']").press("Backspace")
                page.locator("input[name='startDate']").fill(date_str)
                page.locator("input[name='startDate']").press("Enter")
                
                # 2. Bitiş Tarihi
                page.locator("input[name='endDate']").click()
                page.locator("input[name='endDate']").press("Control+a")
                page.locator("input[name='endDate']").press("Backspace")
                page.locator("input[name='endDate']").fill(date_str)
                page.locator("input[name='endDate']").press("Enter") 

                # 3. Sorgula
                page.locator("button:has-text('Sorgula')").click(force=True)
                
                # 4. Bekle
                try:
                    page.wait_for_selector(".epuitable-row-item", state="visible", timeout=10000)
                    time.sleep(1.5)
                except:
                    print(f"⚠️ {date_str} veri gelmedi.")
                    current_date += timedelta(days=1)
                    continue

                # 5. Oku
                rows = page.locator(".epuitable-row-item").all()
                day_count = 0
                for row in rows:
                    try:
                        tarih = row.locator(".epuitable-cell-item-0 span").inner_text()
                        saat = row.locator(".epuitable-cell-item-1 span").inner_text()
                        tuketim = row.locator(".epuitable-cell-item-2 span").inner_text()
                        
                        new_data.append({
                            "Tarih": tarih,
                            "Saat": saat,
                            "Tuketim_MWh": tuketim
                        })
                        day_count += 1
                    except: continue
                
                print(f"  ✅ {day_count} satır alındı.")

            except Exception as e:
                print(f"  ❌ Hata ({date_str}): {e}")
            
            current_date += timedelta(days=1)
            time.sleep(1)

        # ======================================================================
        # 3. BİRLEŞTİRME VE KAYDETME (DÜZELTİLDİ)
        # ======================================================================
        print("\n💾 Kaydediliyor...")
        
        if new_data:
            df_new = pd.DataFrame(new_data)
            
            # Formatlama (Yeni veriyi temizle)
            df_new["Tuketim_MWh"] = df_new["Tuketim_MWh"].astype(str).str.replace(".", "", regex=False).str.replace(",", ".", regex=False)
            df_new["Tuketim_MWh"] = pd.to_numeric(df_new["Tuketim_MWh"], errors="coerce")
            
            # Yeni veriyi datetime objesine çevir
            df_new["DateTime"] = pd.to_datetime(df_new["Tarih"] + " " + df_new["Saat"], format="%d.%m.%Y %H:%M")
            df_new = df_new[["DateTime", "Tuketim_MWh"]]
            
            # Eğer eski dosya varsa birleştir
            if os.path.exists(FINAL_CSV_FILE):
                try:
                    df_old = pd.read_csv(FINAL_CSV_FILE)
                    
                    # --- KRİTİK DÜZELTME BURADA ---
                    # Eski dosyadaki tarihleri de yazıdan (str) gerçek tarihe (datetime) çeviriyoruz
                    df_old["DateTime"] = pd.to_datetime(df_old["DateTime"])
                    # ------------------------------

                    # Yeni veriyi ekle
                    df_final = pd.concat([df_old, df_new])
                    
                    # DateTime'a göre tekrar edenleri temizle (Son geleni tut)
                    df_final = df_final.drop_duplicates(subset="DateTime", keep="last")
                except Exception as e:
                    print(f"⚠️ Eski dosya okunurken hata oluştu, sadece yeni veriler yazılıyor. Hata: {e}")
                    df_final = df_new
            else:
                df_final = df_new

            # Sırala ve Kaydet (Artık ikisi de tarih olduğu için hata vermez)
            df_final = df_final.sort_values("DateTime")
            
            df_final.to_csv(FINAL_CSV_FILE, index=False)
            print(f"🎉 Dosya güncellendi: {FINAL_CSV_FILE}")
            print(f"Toplam Satır: {len(df_final)}")
        else:
            print("✅ Yeni veri bulunamadı, dosya güncel.")
            
        browser.close()

if __name__ == "__main__":
    run()