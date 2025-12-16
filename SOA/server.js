
const express = require('express'); 
const dotenv = require('dotenv'); 

dotenv.config(); 

const app = express();


app.use(express.json()); 

app.use('/api/auth', require('./routes/auth')); 
app.use('/api/users', require('./routes/users')); 
app.use('/api/prediction', require('./routes/prediction')); 

const PORT = process.env.API_PORT || 5001;

app.listen(PORT, () => console.log(`🚀 SOA Sunucusu http://localhost:${PORT} adresinde çalışıyor...`));