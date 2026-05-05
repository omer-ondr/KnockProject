// --- 1. DAKTİLO (TYPEWRITER) MOTORU ---
function typeWriterEffect(text, element, speed = 40, onComplete = null) {
    if (!element) return;

    element.innerHTML = ""; // Önceki yazıyı temizle
    let i = 0;

    // AI'dan gelen "\n" metinlerini gerçek alt satır karakterine çevir
    const cleanText = text.replace(/\\n/g, '\n');

    function type() {
        if (i < cleanText.length) {
            let char = cleanText.charAt(i);

            // Eğer karakter alt satıra geçme komutuysa, HTML <br> etiketi bas
            if (char === '\n') {
                element.innerHTML += "<br>";
            } else {
                element.innerHTML += char;
            }

            i++;
            // Robotikliği kırmak için rastgele insan payı
            let randomSpeed = speed + (Math.random() * 30 - 10);
            setTimeout(type, randomSpeed);
        } else {
            // Yazma işlemi bittiğinde tetiklenecek olay (İndirme butonunu göstermek için)
            if (typeof onComplete === "function") {
                onComplete();
            }
        }
    }

    type();
}

// --- 2. ANA UYGULAMA MANTIĞI ---
document.addEventListener('DOMContentLoaded', () => {
    // DOM Elementleri
    const interactionArea = document.getElementById('interaction-area');
    const memoryWall = document.getElementById('memory-wall');
    const submitBtn = document.getElementById('submit-btn');
    const resetBtn = document.getElementById('reset-btn');
    const farewellText = document.getElementById('farewell-text');
    const loadingSection = document.getElementById('loading');
    const loadingStatus = document.getElementById('loading-status');
    const resultBadge = document.getElementById('result-badge');
    const resultEpigraph = document.getElementById('result-epigraph');
    const inputSection = document.querySelector('.input-section');
    const musicPlayerContainer = document.getElementById('music-player-container');
    const youtubePlayer = document.getElementById('youtube-player');
    const downloadBtn = document.getElementById('download-btn');

    // SignalR Bağlantısı
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://knock-project-api.onrender.com/progressHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveProgress", (message) => {
        if (loadingStatus) loadingStatus.textContent = message;
    });

    connection.start()
        .then(() => console.log("SignalR Bağlantısı Kuruldu."))
        .catch(err => console.error("SignalR Hatası: ", err));

    // Form Gönderimi (1973'e Bağlan)
    submitBtn.addEventListener('click', async () => {
        const text = farewellText.value.trim();
        if (!text) return;

        // UI Durumu: Yükleniyor
        inputSection.classList.add('hidden');
        loadingSection.classList.remove('hidden');
        if (downloadBtn) downloadBtn.classList.add('hidden');
        if (loadingStatus) loadingStatus.textContent = '1973 yılına bağlanılıyor...';

        try {
            // API İsteği
            const response = await fetch('https://knock-project-api.onrender.com/api/farewell', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ message: text, connectionId: connection.connectionId })
            });

            if (!response.ok) throw new Error(`API Hatası: ${response.status}`);

            const data = await response.json();

            // UI Durumu: Başarılı
            loadingSection.classList.add('hidden');
            interactionArea.classList.add('hidden');

            // Görseli Ayarla
            if (data.badgeImage.startsWith('http') || data.badgeImage.startsWith('data:image')) {
                resultBadge.src = data.badgeImage;
            } else if (data.badgeImage === "image_generation_unavailable") {
                resultBadge.src = "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='512' height='512' viewBox='0 0 512 512'%3E%3Crect width='512' height='512' fill='%231a1a1a'/%3E%3Ctext x='256' y='256' text-anchor='middle' dominant-baseline='central' font-family='serif' font-size='24' fill='%23777'%3EImage Unavailable%3C/text%3E%3C/svg%3E";
            } else {
                resultBadge.src = `data:image/png;base64,${data.badgeImage}`;
            }

            // Daktilo Motorunu Başlat (Yanındaki tırnak işaretlerini estetik durması için korudum)
            const finalEpigraph = `"${data.epigraph}"`;
            typeWriterEffect(finalEpigraph, resultEpigraph, 40, () => {
                if (downloadBtn) downloadBtn.classList.remove('hidden');
            });

            // YouTube Müzik Oynatıcı
            if (data.musicTrack && data.musicTrack.videoUrl) {
                const urlParams = new URLSearchParams(new URL(data.musicTrack.videoUrl).search);
                const videoId = urlParams.get('v');
                if (videoId) {
                    youtubePlayer.src = `https://www.youtube.com/embed/${videoId}?autoplay=1&controls=1`;
                    musicPlayerContainer.classList.remove('hidden');
                } else {
                    musicPlayerContainer.classList.add('hidden');
                }
            } else {
                musicPlayerContainer.classList.add('hidden');
            }

            memoryWall.classList.remove('hidden');

        } catch (error) {
            console.error("Zaman çizelgesiyle bağlantı kurulamadı.", error);
            alert("1973 ile bağlantı koptu. Lütfen backend API'nin çalıştığından emin olun.");
            loadingSection.classList.add('hidden');
            inputSection.classList.remove('hidden');
        }
    });

    // Sıfırla Butonu
    resetBtn.addEventListener('click', () => {
        memoryWall.classList.add('hidden');
        musicPlayerContainer.classList.add('hidden');
        if (downloadBtn) downloadBtn.classList.add('hidden');
        youtubePlayer.src = '';

        farewellText.value = '';
        interactionArea.classList.remove('hidden');
        inputSection.classList.remove('hidden');
    });


    // Kartpostal İndirme Fonksiyonu
    if (downloadBtn) {
        downloadBtn.addEventListener('click', async () => {
            const memoryWall = document.getElementById('memory-wall');
            const musicPlayer = document.getElementById('music-player-container');
            const buttonGroup = downloadBtn.parentElement;

            // 1. YouTube iframe'i ve butonları DOM'dan geçici olarak SAKLA!
            // display: none bile bazen Safari'yi kandıramaz, o yüzden tamamen siliyoruz gibi yapıp geri getireceğiz.
            const originalMusicDisplay = musicPlayer.style.display;
            const originalButtonDisplay = buttonGroup.style.display;

            musicPlayer.style.display = 'none';
            buttonGroup.style.display = 'none';

            try {
                // allowTaint KESİNLİKLE YOK! Sadece useCORS var.
                const canvas = await html2canvas(memoryWall, {
                    useCORS: true,
                    backgroundColor: '#1a1a1a', // Arka plan siyah
                    scale: 2
                });

                // 2. Resmi bilgisayara indir
                const link = document.createElement('a');
                link.download = '1973-Hatirasi.png';
                link.href = canvas.toDataURL('image/png'); // Zehirli kod gidince burası artık çalışacak!
                link.click();

            } catch (error) {
                console.error("Fotoğraf çekilirken ölümcül hata:", error);
                alert("Tarayıcın bu işleme engel oldu. Chrome kullanmayı deneyebilirsin.");
            } finally {
                // 3. UI'ı anında eski haline getir
                musicPlayer.style.display = originalMusicDisplay;
                buttonGroup.style.display = originalButtonDisplay;
            }
        });
    }
});