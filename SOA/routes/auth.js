const express = require('express');
const router = express.Router();
const pool = require('../db');
const bcrypt = require('bcryptjs');
const nodemailer = require('nodemailer');

require('dotenv').config();

// NOT: Şifreleri kod içine açıkça yazmak güvenli değildir. .env dosyasından çekmen daha sağlıklı olur.
const transporter = nodemailer.createTransport({
    service: 'gmail',
    auth: {
        user: '27metinserinkaya@gmail.com',
        pass: 'pept nxlk dnmv njue' 
    }
});

// 1. REGISTER
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

// 2. LOGIN
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

// 3. FORGOT PASSWORD
router.post('/forgot-password', async (req, res) => {
    const { email } = req.body;

    try {
        const [users] = await pool.execute('SELECT * FROM users WHERE Email = ?', [email]);
        if (users.length === 0) {
            return res.status(404).json({ message: 'Bu e-posta adresi sistemde kayıtlı değil.' });
        }

        const newPassword = Math.random().toString(36).slice(-8);
        const salt = await bcrypt.genSalt(10);
        const hashedPassword = await bcrypt.hash(newPassword, salt);
        
        await pool.execute('UPDATE users SET PasswordHash = ? WHERE Email = ?', [hashedPassword, email]);

        const mailOptions = {
            from: '"Enerji Tahmin Sistemi" <27metinserinkaya@gmail.com>',
            to: email,
            subject: 'Şifre Sıfırlama Talebi',
            text: `Merhaba,\n\nYeni Geçici Şifreniz: ${newPassword}\n\nLütfen giriş yaptıktan sonra şifrenizi değiştirin.`,
            html: `<h3>Merhaba,</h3><p>Hesabınız için yeni şifre oluşturuldu.</p><p><strong>Yeni Şifreniz:</strong> <span style="font-size:18px; color:blue;">${newPassword}</span></p><p>Lütfen giriş yaptıktan sonra profil sayfasından şifrenizi güncelleyin.</p>`
        };

        transporter.sendMail(mailOptions, (error, info) => {
            if (error) {
                console.error('Mail gönderme hatası:', error);
                return res.status(500).json({ message: 'Şifre güncellendi ancak e-posta gönderilemedi.' });
            }
            console.log('E-posta başarıyla gönderildi: ' + info.response);
            res.status(200).json({ message: 'Yeni şifreniz e-posta adresinize gönderildi.' });
        });

    } catch (error) {
        console.error('Sistem hatası:', error);
        res.status(500).json({ message: 'Sunucu hatası oluştu.' });
    }
});

// 4. DELETE ACCOUNT (YENİ EKLENEN KISIM)
router.delete('/delete-account/:id', async (req, res) => {
    const userId = req.params.id; 

    try {
        // SQL Injection'a karşı parametreli sorgu (?) kullanıyoruz
        const [result] = await pool.execute('DELETE FROM users WHERE UserID = ?', [userId]);

        if (result.affectedRows > 0) {
            res.status(200).json({ message: 'Hesap başarıyla silindi.' });
        } else {
            res.status(404).json({ message: 'Kullanıcı bulunamadı.' });
        }
    } catch (error) {
        console.error('Silme Hatası:', error);
        res.status(500).json({ message: 'Sunucu hatası, hesap silinemedi.' });
    }
});


module.exports = router;