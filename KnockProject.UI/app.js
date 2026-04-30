document.addEventListener('DOMContentLoaded', () => {
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

    // SignalR Connection
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5101/progressHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveProgress", (message) => {
        if (loadingStatus) {
            loadingStatus.textContent = message;
        }
    });

    connection.start()
        .then(() => console.log("SignalR Connected."))
        .catch(err => console.error("SignalR Connection Error: ", err));

    submitBtn.addEventListener('click', async () => {
        const text = farewellText.value.trim();
        if (!text) return;

        // UI State: Loading
        inputSection.classList.add('hidden');
        loadingSection.classList.remove('hidden');
        if (loadingStatus) loadingStatus.textContent = 'Zamanda yolculuk başlıyor...';

        try {
            // Actual API Call to Role 1's Backend
            const response = await fetch('http://localhost:5101/api/farewell', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ message: text, connectionId: connection.connectionId })
            });

            if (!response.ok) {
                throw new Error(`API Error: ${response.status}`);
            }

            const data = await response.json();

            // UI State: Success
            loadingSection.classList.add('hidden');
            interactionArea.classList.add('hidden');
            
            // Set image source. Check if it's base64 or direct URL
            if (data.badgeImage.startsWith('http') || data.badgeImage.startsWith('data:image')) {
                resultBadge.src = data.badgeImage;
            } else if (data.badgeImage === "image_generation_unavailable") {
                // Fallback to local image if DALL-E/SDXL failed on backend
                resultBadge.src = "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='512' height='512' viewBox='0 0 512 512'%3E%3Crect width='512' height='512' fill='%231a1a1a'/%3E%3Ctext x='256' y='256' text-anchor='middle' dominant-baseline='central' font-family='serif' font-size='24' fill='%23777'%3EImage Unavailable%3C/text%3E%3C/svg%3E"; 
            } else {
                // Assume base64
                resultBadge.src = `data:image/png;base64,${data.badgeImage}`;
            }

            resultEpigraph.textContent = `"${data.epigraph}"`;
            
            // Handle YouTube Music Player
            if (data.musicTrack && data.musicTrack.videoUrl) {
                // Extract Video ID from e.g. https://www.youtube.com/watch?v=VIDEO_ID
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
            console.error("Failed to connect to the timeline.", error);
            alert("The connection to 1973 was lost. Please ensure the backend API is running.");
            loadingSection.classList.add('hidden');
            inputSection.classList.remove('hidden');
        }
    });

    resetBtn.addEventListener('click', () => {
        memoryWall.classList.add('hidden');
        musicPlayerContainer.classList.add('hidden');
        youtubePlayer.src = ''; // Stop audio playback
        farewellText.value = '';
        interactionArea.classList.remove('hidden');
        inputSection.classList.remove('hidden');
    });
});
