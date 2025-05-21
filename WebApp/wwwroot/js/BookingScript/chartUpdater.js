document.addEventListener("DOMContentLoaded", function () {
    if (!document.getElementById("bookingChart")) return;

    let bookingChart;
    let chartLabels = window.chartLabels || [];
    let chartData = window.chartData || [];

    const ctx = document.getElementById("bookingChart").getContext("2d");

    bookingChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: chartLabels,
            datasets: [{
                data: chartData,
                fill: true,
                tension: 0.4,
                borderColor: '#f0f',
                backgroundColor: 'rgba(240, 0, 240, 0.1)',
                pointRadius: 4,
                pointHoverRadius: 6
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: { precision: 0 }
                }
            }
        }
    });

    document.getElementById("chartRangeSelect").addEventListener("change", async function () {
        const range = this.value;
        try {
            const res = await fetch(`https://bookingserviceventixe-fbb7amdzfsh4b4d6.swedencentral-01.azurewebsites.net/api/bookings/stats/overview?range=${range}`);
            const updated = await res.json().catch(() => ({ labels: [], data: [] }));

            bookingChart.data.labels = updated.labels;
            bookingChart.data.datasets[0].data = updated.data;
            bookingChart.update();
        } catch (err) {
            console.error("Chart update failed:", err);
        }
    });
});