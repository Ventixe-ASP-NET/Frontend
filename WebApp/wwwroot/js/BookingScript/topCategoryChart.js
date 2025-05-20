document.addEventListener("DOMContentLoaded", function () {
    const chartData = window.topCategoryData?.categories || [];
    const canvas = document.getElementById("topCategoriesChart");
    const select = document.getElementById("categoryRangeSelect");
    if (!canvas) return;

    window.topCategoryColors = ['#F26CF9', '#37437D', '#DDDEED', '#EEEFFF'];
    window.topCategoryColors.forEach((color, index) => {
        document.documentElement.style.setProperty(`--color${index}`, color);
    });

    const ctx = canvas.getContext("2d");
    window.topCategoriesChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: chartData.map(c => c.name),
            datasets: [{
                data: chartData.map(c => c.count),
                backgroundColor: window.topCategoryColors
            }]
        },
        options: {
            plugins: { legend: { display: false } },
            cutout: "70%",
            responsive: true,
            maintainAspectRatio: false
        }
    });

    if (select) {
        select.addEventListener("change", async function () {
            const range = this.value;
            try {
                const res = await fetch(`https://bookingeventgateway-f8b4d2ahagc5faev.swedencentral-01.azurewebsites.net/api/bookingwithevents/stats/top-categories?range=${range}`);
                const data = await res.json();
                updateTopCategoryChart(data);
            } catch (err) {
                console.error("Failed to update top categories:", err);
            }
        });
    }

    function updateTopCategoryChart(data) {
        if (!data || !window.topCategoriesChart || !data.categories) return;

        const chart = window.topCategoriesChart;
        chart.data.labels = data.categories.map(c => c.name);
        chart.data.datasets[0].data = data.categories.map(c => c.count);
        chart.update();

        const totalEl = document.querySelector(".total-value");
        if (totalEl) totalEl.textContent = data.totalBookings;

        const barContainer = document.querySelector(".category-bars");
        if (barContainer) {
            barContainer.innerHTML = "";
            data.categories.forEach((cat, index) => {
                const div = document.createElement("div");
                div.className = "category-item";
                div.innerHTML = `
                    <div class="bar-top">
                        <span class="bar-name">${cat.name}</span>
                        <span class="bar-value">${cat.count}</span>
                    </div>
                    <div class="bar-outer">
                        <div class="bar-fill" style="width:${cat.percentage}%; background: var(--color${index});"></div>
                    </div>`;
                barContainer.appendChild(div);
            });
        }
    }

    // ⏱️ Kör direkt efter sidladdning
    if (window.topCategoryData && window.topCategoryData.categories) {
        updateTopCategoryChart(window.topCategoryData);
    }
});