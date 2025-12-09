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

// --- 3. SEPETE EKLEME (BACKEND İLE) ---
async function addToCart(id, name, price, imgIcon) {
    // Kullanıcıya bilgi verelim (İsteğe bağlı)
    // alert("Ekleniyor..."); 

    const formData = new FormData();
    formData.append('urunId', id);
    formData.append('adet', 1);

    try {
        const response = await fetch('/Siparis/SepeteEkle', {
            method: 'POST',
            body: formData
        });

        if (response.status === 401) {
            window.location.href = '/Musteri/Giris'; // Giriş yapılmamışsa yönlendir
            return;
        }

        const result = await response.json();

        if (result.success) {
            alert(result.message); // "Ürün sepete eklendi!"
            loadCart(); // Sepet sayısını hemen güncelle
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

        // Konsoldan kontrol etmek için:
        // console.log("Gelen Veri:", cartItems);

        renderCartItems(cartItems); // HTML Çiz
        updateCartCount(cartItems); // Sayacı Güncelle

    } catch (error) {
        console.error("Sepet yüklenirken hata:", error);
    }
}

// --- 5. SEPETİ EKRANA ÇİZME (RENDER) ---
// --- 5. SEPETİ EKRANA ÇİZME (RENDER) ---
// --- 5. SEPETİ EKRANA ÇİZME (RENDER) ---
// --- 5. SEPETİ EKRANA ÇİZME (RENDER) ---
function renderCartItems(cartItems) {
    const container = document.getElementById('cartItemsContainer');
    const totalEl = document.getElementById('cartTotal');

    // --- YENİ EKLENEN KISIM: Siparişi Tamamla Butonunu Bul ---
    // Butonun Layout dosyasındaki class'ı veya yeri bellidir.
    // Genelde footer kısmında durur. Onu bulup onclick ekliyoruz.
    const checkoutBtn = document.querySelector('.cart-footer .hypr-btn-success');
    if (checkoutBtn) {
        // Eski event listener'ları temizlemek için klonlama yöntemi veya direkt atama:
        checkoutBtn.onclick = completeOrder;
    }
    // ---------------------------------------------------------

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
            // Null/Undefined kontrolleri (Küçük harf ile özellikleri alıyoruz)
            const fiyat = item.fiyat || 0;
            const adet = item.adet || 0;
            const ad = item.urunAdi || "İsimsiz Ürün";
            const resim = item.resimUrl || "";

            // --- RESİM URL MANTIĞI (DÜZELTİLDİ) ---
            let finalResimSrc = "";

            if (!resim) {
                // Resim yoksa boş
                finalResimSrc = "";
            } else if (resim.toLowerCase().startsWith('http')) {
                // Eğer link 'http' ile başlıyorsa (Tam linkse) dokunma
                finalResimSrc = resim;
            } else {
                // Normal dosya ismiyse başına /img/ ekle
                finalResimSrc = `/img/${resim}`;
            }

            // HTML Resim Etiketi
            const imgHtml = finalResimSrc
                ? `<img src="${finalResimSrc}" style="width:100%; height:100%; object-fit:cover; border-radius:6px;">`
                : '<span style="font-size:1.5rem">📦</span>';
            // ----------------------------------------

            // Toplam Hesapla
            const itemTotal = fiyat * adet;
            totalPrice += itemTotal;

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
    if (!cartItems) cartItems = []; // Boş gelirse patlamasın

    const totalQty = cartItems.reduce((acc, item) => acc + (item.adet || 0), 0);
    const btn = document.getElementById('openCartBtn');
    if (btn) btn.innerText = `Sepet (${totalQty})`;
}

// ==========================================
// SAYFA YÜKLENDİĞİNDE ÇALIŞACAKLAR (MAIN)
// ==========================================
document.addEventListener('DOMContentLoaded', () => {

    // A. Sepeti veritabanından getir
    loadCart();

    // B. Sepet Butonuna Tıklama Olayı
    const openCartBtn = document.getElementById('openCartBtn');
    if (openCartBtn) {
        // Eski event listener'ları temizlemek yerine doğru fonksiyonu bağlıyoruz
        openCartBtn.onclick = toggleCart;
    }

    // C. Arama Fonksiyonu
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

    // E. Auth Butonu (Logout İşlemi)
    const authBtn = document.getElementById('authBtn');
    // Eğer buton varsa ve içinde "Çıkış" yazıyorsa logout işlemini bağla
    // (Layout'ta href ile halletmiş olabiliriz ama JS kontrolü istersen buraya eklenir)
});
// --- ADET DEĞİŞTİRME FONKSİYONU (+ / -) ---
async function changeQty(urunId, degisim) {

    // Veriyi hazırla
    const formData = new FormData();
    formData.append('urunId', urunId);
    formData.append('degisim', degisim); // +1 veya -1

    try {
        // Backend'e gönder
        const response = await fetch('/Siparis/SepetGuncelle', {
            method: 'POST',
            body: formData
        });

        const result = await response.json();

        if (result.success) {
            // BAŞARILI: Sepeti veritabanından tekrar çek (Böylece yeni sayı ve fiyat görünür)
            loadCart();
        } else {
            console.error("Güncelleme başarısız:", result.message);
        }

    } catch (error) {
        console.error("Hata:", error);
    }
}
// --- SİPARİŞİ TAMAMLA (CHECKOUT) ---
// --- SİPARİŞİ TAMAMLA (CHECKOUT) ---
async function completeOrder() {
    // Sepet boşsa işlem yapma
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
            // Başarılıysa Teşekkürler sayfasına git
            window.location.href = result.redirectUrl;
        } else {
            alert("Hata: " + result.message);
        }
    } catch (error) {
        console.error("Sipariş hatası:", error);
        alert("Bir sorun oluştu.");
    }
}