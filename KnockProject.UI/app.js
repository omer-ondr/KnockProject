document.addEventListener('DOMContentLoaded', () => {
    const interactionArea = document.getElementById('interaction-area');
    const memoryWall = document.getElementById('memory-wall');
    const submitBtn = document.getElementById('submit-btn');
    const resetBtn = document.getElementById('reset-btn');
    const farewellText = document.getElementById('farewell-text');
    const loadingSection = document.getElementById('loading');
    const resultBadge = document.getElementById('result-badge');
    const resultEpigraph = document.getElementById('result-epigraph');
    const inputSection = document.querySelector('.input-section');

    submitBtn.addEventListener('click', async () => {
        const text = farewellText.value.trim();
        if (!text) return;

        // UI State: Loading
        inputSection.classList.add('hidden');
        loadingSection.classList.remove('hidden');

        try {
            // Actual API Call to Role 1's Backend
            const response = await fetch('http://localhost:5101/api/farewell', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ message: text }) // Matches 'FarewellRequest'
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
                resultBadge.src = "badge_placeholder.png"; 
            } else {
                // Assume base64
                resultBadge.src = `data:image/png;base64,${data.badgeImage}`;
            }

            resultEpigraph.textContent = `"${data.epigraph}"`;
            
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
        farewellText.value = '';
        interactionArea.classList.remove('hidden');
        inputSection.classList.remove('hidden');
    });
});
