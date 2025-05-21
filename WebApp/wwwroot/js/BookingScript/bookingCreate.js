document.addEventListener("DOMContentLoaded", () => {
    // Toggle booking form
    document.querySelectorAll(".book-now-btn").forEach(button => {
        button.addEventListener("click", () => {
            const card = button.closest(".event-card");
            const form = card.querySelector(".booking-form");
            form.style.display = form.style.display === "none" ? "flex" : "none";
        });
    });

    // Submit booking
    document.querySelectorAll(".submit-booking-btn").forEach(button => {
        button.addEventListener("click", async () => {
            const form = button.closest(".booking-form");
            const eventId = form.getAttribute("data-event-id");
            const ticketSelect = form.querySelector(".ticket-type-dropdown");
            const quantityInput = form.querySelector(".ticket-amount");
            const errorEl = form.querySelector(".booking-error");

            const selectedOption = ticketSelect.options[ticketSelect.selectedIndex];
            const ticketTypeId = ticketSelect.value;
            const ticketTypeName = selectedOption?.dataset.name;
            const ticketsLeft = parseInt(selectedOption?.dataset.left);
            const price = parseFloat(selectedOption?.dataset.price);
            const quantity = parseInt(quantityInput.value);

            errorEl.textContent = "";

            if (!ticketTypeId || isNaN(quantity) || quantity < 1) {
                errorEl.textContent = "Please select a ticket type and quantity.";
                return;
            }

            if (quantity > ticketsLeft) {
                errorEl.textContent = "Not enough tickets left.";
                return;
            }

            const payload = {
                invoiceId: "INV" + Math.floor(Math.random() * 100000),
                bookingName: "User123", // Byt ut mot inloggad användare sen
                createdAt: new Date().toISOString(),
                eventId: eventId,
                tickets: [
                    {
                        ticketTypeId,
                        ticketType: ticketTypeName,
                        quantity,
                        pricePerTicket: price
                    }
                ]
            };

            try {
                const res = await fetch("https://bookingserviceventixe-fbb7amdzfsh4b4d6.swedencentral-01.azurewebsites.net/api/bookings", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(payload)
                });

                const text = await res.text();

                if (!res.ok) {
                    errorEl.textContent = "Booking failed: " + text;
                    return;
                }

                alert("Booking successful!");
                form.style.display = "none";
                quantityInput.value = "";
                ticketSelect.selectedIndex = 0;
            } catch (err) {
                errorEl.textContent = "Could not reach server. Check network.";
                console.error(err);
            }
        });
    });
});
