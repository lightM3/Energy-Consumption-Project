const express = require('express');
const router = express.Router();
const pool = require('../db');
const bcrypt = require('bcryptjs'); // Şifre güncelleme için gerekli

// 1. PROFİL BİLGİLERİNİ GETİR (GET)
router.get('/profile/:userId', async (req, res) => {
    try {
        const userId = req.params.userId; 
        
        const [users] = await pool.execute(
            'SELECT UserID, FullName, Email FROM users WHERE UserID = ?', 
            [userId]
        );
        
        const user = users[0];

        if (!user) {
            return res.status(404).json({ message: 'Kullanıcı bulunamadı.' });
        }

        res.status(200).json(user);

    } catch (error) {
        console.error("Profil Rotası Hatası:", error);
        res.status(500).json({ message: 'Sunucu hatası.' });
    }
});

// 2. PROFİL GÜNCELLE (PUT) - EKSİK OLAN KISIM BURASIYDI
router.put('/update-profile', async (req, res) => {
    const { userId, name, password } = req.body;

    try {
        // Kullanıcı var mı kontrol et
        const [users] = await pool.execute('SELECT * FROM users WHERE UserID = ?', [userId]);
        if (users.length === 0) {
            return res.status(404).json({ message: 'Kullanıcı bulunamadı.' });
        }

        // SENARYO 1: Şifre alanı doluysa (Hem isim hem şifre güncelle)
        if (password && password.trim() !== "") {
            const salt = await bcrypt.genSalt(10);
            const hashedPassword = await bcrypt.hash(password, salt);

            await pool.execute(
                'UPDATE users SET FullName = ?, PasswordHash = ? WHERE UserID = ?',
                [name, hashedPassword, userId]
            );
        } 
        // SENARYO 2: Şifre boşsa (Sadece ismi güncelle)
        else {
            await pool.execute(
                'UPDATE users SET FullName = ? WHERE UserID = ?',
                [name, userId]
            );
        }

        res.status(200).json({ message: 'Profil başarıyla güncellendi.' });

    } catch (error) {
        console.error('Güncelleme Hatası:', error);
        res.status(500).json({ message: 'Sunucu hatası, güncelleme yapılamadı.' });
    }
});

module.exports = router;