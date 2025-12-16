
const express = require('express');
const router = express.Router();
const pool = require('../db');


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

module.exports = router;