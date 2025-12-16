
const express = require('express');
const soap = require('soap');
const fs = require('fs');
const path = require('path');
const fetch = require('node-fetch');
const http = require('http'); 

const app = express();
const SOAP_PORT = 5003;
const WSDL_PATH = path.join(__dirname, 'tahmin_servisi.wsdl'); 
const ML_API_URL = process.env.ML_API_URL || 'http://localhost:5000/predict'; 


app.use(express.text({ type: 'text/xml' }));

const tahminService = {
    TahminService: {
        TahminPort: {
            TahminYap: async (args) => {
                const { tarih, saat, sicaklik, lag_24h } = args;

                try {
                  
                    const mlResponse = await fetch(ML_API_URL, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ 
                            tarih: tarih, 
                            saat: parseInt(saat), 
                            sicaklik: parseFloat(sicaklik), 
                            lag_24h: parseFloat(lag_24h) 
                        })
                    });
                    
                    const mlData = await mlResponse.json();

                    if (mlData.status !== 'success') {
                        throw new Error(mlData.mesaj || 'ML API hata döndürdü.');
                    }

                   
                    return {
                        tahmin_mwh: mlData.tahmin_mwh,
                        mesaj: 'Tahmin başarılı (SOAP üzerinden).'
                    };
                } catch (error) {
                    console.error("SOAP Tahmin Hatası:", error);
                    
                    throw {
                        Fault: {
                            faultcode: 'Server',
                            faultstring: `İşlem hatası: ${error.message}`
                        }
                    };
                }
            }
        }
    }
};


const wsdl = fs.readFileSync(WSDL_PATH, 'utf8');

const httpServer = http.createServer(app);


soap.listen(httpServer, '/tahmin', tahminService, wsdl);

httpServer.listen(SOAP_PORT, () => {
    console.log(`📡 SOAP Sunucusu http://localhost:${SOAP_PORT}/tahmin adresinde çalışıyor. (WSDL: http://localhost:${SOAP_PORT}/tahmin?wsdl)`);
});