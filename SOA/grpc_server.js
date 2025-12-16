
const grpc = require('@grpc/grpc-js');
const protoLoader = require('@grpc/proto-loader');
const fetch = require('node-fetch');
const path = require('path');

const GRPC_PORT = 5004;
const ML_API_URL = process.env.ML_API_URL || 'http://localhost:5000/predict'; 


const PROTO_PATH = path.join(__dirname, 'tahmin.proto');
const packageDefinition = protoLoader.loadSync(
    PROTO_PATH,
    {keepCase: true, longs: String, enums: String, defaults: true, oneofs: true}
);
const tahmin_proto = grpc.loadPackageDefinition(packageDefinition).tahmin;


async function TahminYap(call, callback) {
    const { tarih, saat, sicaklik, lag_24h } = call.request;

    try {
        
        const mlResponse = await fetch(ML_API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ tarih, saat, sicaklik, lag_24h })
        });
        
        const mlData = await mlResponse.json();

        if (mlData.status !== 'success') {
            return callback({ code: grpc.status.INTERNAL, message: mlData.mesaj || 'ML API hata döndürdü.' });
        }

       
        callback(null, {
            tahmin_mwh: mlData.tahmin_mwh,
            mesaj: 'Tahmin başarılı (gRPC üzerinden).'
        });

    } catch (error) {
        console.error("gRPC Tahmin Hatası:", error);
        callback({ code: grpc.status.UNAVAILABLE, message: `Sunucu hatası: ${error.message}` });
    }
}


function main() {
    const server = new grpc.Server();
    server.addService(tahmin_proto.TahminService.service, { TahminYap: TahminYap });
    
    server.bindAsync(`0.0.0.0:${GRPC_PORT}`, grpc.ServerCredentials.createInsecure(), () => {
        server.start();
        console.log(`✨ gRPC sunucusu 0.0.0.0:${GRPC_PORT} adresinde çalışıyor...`);
    });
}

main();