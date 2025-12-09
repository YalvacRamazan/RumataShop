// ==========================================
// RUMATA SHOP - MODERN JAVASCRIPT DOSYASI
// ==========================================

// --- 1. KATEGORİ SIDEBAR ---
function toggleCategorySidebar() {
    const sidebar = document.getElementById('categorySidebar');
    const overlay = document.getElementById('categoryOverlay');
    if (sidebar && overlay) {
        sidebar.classList.toggle('open');
        overlay.classList.toggle('show');
    }
}

// --- 2. SEPET SIDEBAR AÇMA/KAPAMA ---
function toggleCart() {
    const sidebar = document.getElementById('cartSidebar');
    const overlay = document.getElementById('cartOverlay');

    if (sidebar && overlay) {
        sidebar.classList.toggle('open');
        overlay.classList.toggle('show');

        // Eğer sepet açılıyorsa, güncel veriyi veritabanından çek
        if (sidebar.classList.contains('open')) {
            loadCart();
        }
    }
}

// --- 3. SEPETE EKLEME (VİTRİNDEN) ---
// Sadece urunId yeterli, adet varsayılan 1 gönderilir.
async function addToCart(urunId) {
    const formData = new FormData();
    formData.append('urunId', urunId);
    formData.append('adet', 1);

    try {
        const response = await fetch('/Siparis/SepeteEkle', {
            method: 'POST',
            body: formData
        });

        if (response.status === 401) {
            // Giriş yapılmamışsa login sayfasına yönlendir
            window.location.href = '/Musteri/Giris';
            return;
        }

        const result = await response.json();

        if (result.success) {
            // Başarılı olursa sepeti güncelle ve aç
            loadCart();
            toggleCart();
        } else {
            alert("Hata: " + result.message);
        }

    } catch (error) {
        console.error('Hata:', error);
        alert("Bir sorun oluştu.");
    }
}

// --- 4. VERİTABANINDAN SEPETİ ÇEKME (LOAD) ---
async function loadCart() {
    try {
        const response = await fetch('/Siparis/SepetiGetir');
        if (!response.ok) return;

        const cartItems = await response.json();
        renderCartItems(cartItems); // HTML Çiz
        updateCartCount(cartItems); // Sayacı Güncelle

    } catch (error) {
        console.error("Sepet yüklenirken hata:", error);
    }
}

// --- 5. SEPETİ EKRANA ÇİZME (RENDER) ---
function renderCartItems(cartItems) {
    const container = document.getElementById('cartItemsContainer');
    const totalEl = document.getElementById('cartTotal');

    // Siparişi Tamamla Butonuna Olay Atama
    const checkoutBtn = document.querySelector('.cart-footer .hypr-btn-success');
    if (checkoutBtn) {
        checkoutBtn.onclick = completeOrder;
    }

    // Eğer container yoksa (başka sayfadaysak) dur
    if (!container || !totalEl) return;

    container.innerHTML = '';
    let totalPrice = 0;

    // Sepet boş kontrolü
    if (!cartItems || cartItems.length === 0) {
        container.innerHTML = '<div style="text-align: center; color: var(--text-muted); margin-top: 50px;">Sepetiniz boş.</div>';
        totalEl.innerText = '₺0,00';
    } else {
        cartItems.forEach(item => {
            const fiyat = item.fiyat || 0;
            const adet = item.adet || 0;
            const ad = item.urunAdi || "İsimsiz Ürün";
            const resim = item.resimUrl || "";

            // --- RESİM URL MANTIĞI ---
            let finalResimSrc = "";
            if (!resim) {
                finalResimSrc = "";
            } else if (resim.toLowerCase().startsWith('http')) {
                finalResimSrc = resim;
            } else {
                finalResimSrc = `/img/${resim}`;
            }

            const imgHtml = finalResimSrc
                ? `<img src="${finalResimSrc}" style="width:100%; height:100%; object-fit:cover; border-radius:6px;">`
                : '<span style="font-size:1.5rem">📦</span>';

            // Toplam Hesapla
            totalPrice += (fiyat * adet);

            // Kutuyu Oluştur
            const div = document.createElement('div');
            div.className = 'cart-item';

            div.innerHTML = `
                <div class="cart-item-img">${imgHtml}</div>
                <div style="flex:1;">
                    <div style="font-weight:600; font-size:0.9rem;">${ad}</div>
                    <div style="color:var(--accent-primary); font-size:0.85rem;">₺${fiyat.toLocaleString('tr-TR', { minimumFractionDigits: 2 })}</div>
                </div>
                <div style="display:flex; align-items:center; gap:8px;">
                    <button class="qty-btn" onclick="changeQty(${item.urunId}, -1)" 
                            style="background:rgba(255,255,255,0.1); border:none; color:white; width:25px; height:25px; border-radius:4px; cursor:pointer;">-</button>
                    
                    <span style="font-weight:bold; color: var(--text-main);">${adet}</span>
                    
                    <button class="qty-btn" onclick="changeQty(${item.urunId}, 1)"
                            style="background:rgba(255,255,255,0.1); border:none; color:white; width:25px; height:25px; border-radius:4px; cursor:pointer;">+</button>
                </div>
            `;
            container.appendChild(div);
        });

        // Genel Toplamı Yaz
        totalEl.innerText = '₺' + totalPrice.toLocaleString('tr-TR', { minimumFractionDigits: 2 });
    }
}

// --- 6. SEPET SAYACINI GÜNCELLEME ---
function updateCartCount(cartItems) {
    if (!cartItems) cartItems = [];
    const totalQty = cartItems.reduce((acc, item) => acc + (item.adet || 0), 0);
    const btn = document.getElementById('openCartBtn');
    if (btn) btn.innerText = `Sepet (${totalQty})`;
}

// --- 7. ADET DEĞİŞTİRME FONKSİYONU (+ / -) ---
async function changeQty(urunId, degisim) {
    const formData = new FormData();
    formData.append('urunId', urunId);
    formData.append('degisim', degisim);

    try {
        const response = await fetch('/Siparis/SepetGuncelle', {
            method: 'POST',
            body: formData
        });

        const result = await response.json();

        if (result.success) {
            loadCart();
        } else {
            console.error("Güncelleme başarısız:", result.message);
        }
    } catch (error) {
        console.error("Hata:", error);
    }
}

// --- 8. SİPARİŞİ TAMAMLA (CHECKOUT) ---
async function completeOrder() {
    const totalEl = document.getElementById('cartTotal');
    if (totalEl && totalEl.innerText === '₺0,00') {
        alert("Sepetiniz boş!");
        return;
    }

    if (!confirm("Siparişi onaylıyor musunuz?")) return;

    try {
        const response = await fetch('/Siparis/SiparisiTamamla', {
            method: 'POST'
        });

        const result = await response.json();

        if (result.success) {
            window.location.href = result.redirectUrl;
        } else {
            alert("Hata: " + result.message);
        }
    } catch (error) {
        console.error("Sipariş hatası:", error);
        alert("Bir sorun oluştu.");
    }
}

// ==========================================
// SAYFA YÜKLENDİĞİNDE ÇALIŞACAKLAR (MAIN)
// ==========================================
document.addEventListener('DOMContentLoaded', () => {

    // A. Sepeti Yükle
    loadCart();

    // B. Sepet Butonuna Tıklama Olayı
    const openCartBtn = document.getElementById('openCartBtn');
    if (openCartBtn) {
        openCartBtn.onclick = toggleCart;
    }

    // C. Arama Fonksiyonu (Vitrin İçin)
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', (e) => {
            const term = e.target.value.toLowerCase();
            const cols = document.querySelectorAll('.product-item-col');
            cols.forEach(col => {
                const titleEl = col.querySelector('.hypr-card-title');
                if (titleEl) {
                    const title = titleEl.innerText.toLowerCase();
                    col.style.display = title.includes(term) ? 'block' : 'none';
                }
            });
        });
    }

    // D. Tema Yönetimi
    const themeBtn = document.getElementById('themeBtn');
    const themes = ['grey', 'white', 'black'];
    let currentThemeIndex = parseInt(localStorage.getItem('themeIndex')) || 0;

    function applyTheme(index) {
        document.body.className = '';
        if (themes[index] === 'white') {
            document.body.classList.add('theme-white');
            if (themeBtn) themeBtn.innerText = '☀️';
        } else if (themes[index] === 'black') {
            document.body.classList.add('theme-black');
            if (themeBtn) themeBtn.innerText = '🌙';
        } else {
            if (themeBtn) themeBtn.innerText = '☁️';
        }
        localStorage.setItem('themeIndex', index);
    }

    applyTheme(currentThemeIndex);

    if (themeBtn) {
        themeBtn.addEventListener('click', () => {
            currentThemeIndex = (currentThemeIndex + 1) % themes.length;
            applyTheme(currentThemeIndex);
        });
    }
});