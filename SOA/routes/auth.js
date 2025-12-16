
const express = require('express');
const router = express.Router();
const pool = require('../db');
const bcrypt = require('bcryptjs');

require('dotenv').config();

router.post('/register', async (req, res) => {
    const { name, email, password } = req.body; 
    try {
        const [users] = await pool.execute('SELECT * FROM users WHERE Email = ?', [email]);
        if (users.length > 0) return res.status(400).json({ message: 'Bu e-posta adresi zaten kayıtlı.' }); 

        const salt = await bcrypt.genSalt(10);
        const hashedPassword = await bcrypt.hash(password, salt);
        
        
        await pool.execute(
            'INSERT INTO users (FullName, Email, PasswordHash) VALUES (?, ?, ?)',
            [name, email, hashedPassword]
        );

        res.status(201).json({ message: 'Kayıt başarılı.' });
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası.' });
    }
});

router.post('/login', async (req, res) => {
    const { email, password } = req.body; 
    try {
        const [users] = await pool.execute('SELECT * FROM users WHERE Email = ?', [email]);
        const user = users[0];

        
        if (!user || !(await bcrypt.compare(password, user.PasswordHash))) {
            return res.status(401).json({ message: 'E-posta veya şifre hatalı.' }); 
        }

       
        res.status(200).json({
            message: 'Giriş başarılı.',
            user: {
                id: user.UserID, 
                name: user.FullName, 
                email: user.Email
            }
        });

    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası.' });
    }
});

module.exports = router;