
const express = require('express');
const router = express.Router();
const pool = require('../db'); 
const fetch = require('node-fetch'); 
require('dotenv').config();

const ML_API_URL = process.env.ML_API_URL || 'http://localhost:5000/predict'; 


router.post('/tahmin', async (req, res) => {
    const { tarih, saat, sicaklik } = req.body;
    
    
    const kullaniciID = 1; 

    if (!tarih || !saat || !sicaklik) {
        return res.status(400).json({ message: 'Eksik tahmin parametreleri (tarih, saat, sicaklik) var.' });
    }

    let lag_24h_value;
    
    try {
      
        const [lagResult] = await pool.execute(
            
            'SELECT Lag_24h FROM electricityconsumptions ORDER BY ConsumptionID DESC LIMIT 1' 
        );

        if (lagResult.length === 0) {
            lag_24h_value = 0.0; 
        } else {
            lag_24h_value = lagResult[0].Lag_24h; 
        }

        const mlResponse = await fetch(ML_API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ 
                tarih: tarih, 
                saat: saat, 
                sicaklik: sicaklik, 
                lag_24h: lag_24h_value 
            }) 
        });

        const mlData = await mlResponse.json();
        const tahmin_mwh = mlData.tahmin_mwh;
        
        if (mlData.status === 'error' || !mlResponse.ok) {
            return res.status(502).json({ 
                status: 'error', 
                message: 'Harici Tahmin Servisi Hatası.',
                detail: mlData.mesaj || mlData.hata_detayi
            });
        }
        
        
        await pool.execute(
            'INSERT INTO predictions (TargetDate, TargetTime, PredictedValue, CreatedDate) VALUES (?, ?, ?, NOW())',
            [tarih, saat, tahmin_mwh] 
        );

   
        res.status(200).json({
            status: 'success',
            tahmin_mwh: tahmin_mwh,
            message: 'Tahmin başarılı, veritabanına kaydedildi.'
        });

    } catch (error) {
        console.error("Tahmin İşlemi/Veritabanı Hatası:", error);
        res.status(500).json({ status: 'error', message: 'Sunucu tarafında beklenmedik bir hata oluştu.' });
    }
});

module.exports = router;