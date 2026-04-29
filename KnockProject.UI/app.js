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
            // Replace with actual API endpoint later
            // const response = await fetch('/api/farewell', {
            //     method: 'POST',
            //     headers: { 'Content-Type': 'application/json' },
            //     body: JSON.stringify({ farewell: text })
            // });
            // const data = await response.json();

            // Mock Data for Demo Purposes
            const data = await mockApiCall(text);

            // UI State: Success
            loadingSection.classList.add('hidden');
            interactionArea.classList.add('hidden');
            
            resultBadge.src = data.imageUrl;
            resultEpigraph.textContent = `"${data.epigraph}"`;
            
            memoryWall.classList.remove('hidden');
            
        } catch (error) {
            console.error("Failed to connect to the timeline.", error);
            alert("The connection to 1973 was lost. Please try again.");
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

    // Fake API call to simulate network delay and return mock data
    function mockApiCall(text) {
        return new Promise(resolve => {
            setTimeout(() => {
                resolve({
                    epigraph: "You laid your gun down on the riverbank, but the river keeps flowing without you.",
                    imageUrl: "badge_placeholder.png"
                });
            }, 2500);
        });
    }
});
