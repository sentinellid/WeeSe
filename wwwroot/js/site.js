// WeeSe - Site JavaScript
// ======================

// Aspetta che jQuery sia caricato
window.addEventListener('DOMContentLoaded', function () {
    console.log('WeeSe loaded successfully!');

    // Se jQuery è disponibile
    if (typeof $ !== 'undefined') {
        $(document).ready(function () {
            console.log('jQuery ready!');

            // Animazioni hover per le card del dashboard
            $('.module-card').hover(
                function () {
                    $(this).addClass('shadow-lg');
                },
                function () {
                    $(this).removeClass('shadow-lg');
                }
            );

            // Animazioni hover per le stat cards
            $('.stat-card').hover(
                function () {
                    $(this).addClass('shadow-lg');
                },
                function () {
                    $(this).removeClass('shadow-lg');
                }
            );

            // Smooth scroll per link interni
            $('a[href^="#"]').on('click', function (event) {
                var target = $(this.getAttribute('href'));
                if (target.length) {
                    event.preventDefault();
                    $('html, body').stop().animate({
                        scrollTop: target.offset().top - 100
                    }, 1000);
                }
            });

            // Auto-hide alerts dopo 5 secondi
            $('.alert').delay(5000).fadeOut();

            // Loading effect sui form submit
            $('form').on('submit', function () {
                var submitBtn = $(this).find('button[type="submit"]');
                var originalText = submitBtn.html();
                submitBtn.html('<i class="fas fa-spinner fa-spin"></i> Caricamento...');
                submitBtn.prop('disabled', true);

                // Ripristina dopo 5 secondi in caso di errore
                setTimeout(function () {
                    submitBtn.html(originalText);
                    submitBtn.prop('disabled', false);
                }, 5000);
            });
        });
    } else {
        console.warn('jQuery non caricato, usando JavaScript vanilla');

        // Fallback con JavaScript vanilla
        const moduleCards = document.querySelectorAll('.module-card');
        moduleCards.forEach(card => {
            card.addEventListener('mouseenter', function () {
                this.classList.add('shadow-lg');
            });
            card.addEventListener('mouseleave', function () {
                this.classList.remove('shadow-lg');
            });
        });

        const statCards = document.querySelectorAll('.stat-card');
        statCards.forEach(card => {
            card.addEventListener('mouseenter', function () {
                this.classList.add('shadow-lg');
            });
            card.addEventListener('mouseleave', function () {
                this.classList.remove('shadow-lg');
            });
        });
    }
});

// Funzione per test connessione DB
function testDbConnection() {
    fetch('/Account/TestDb')
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert('✅ ' + data.message);
            } else {
                alert('❌ ' + data.message);
            }
        })
        .catch(error => {
            alert('❌ Errore: ' + error);
        });
}

// Funzioni di utilità
window.WeeSe = {
    showLoading: function () {
        if (!document.getElementById('loadingOverlay')) {
            const overlay = document.createElement('div');
            overlay.id = 'loadingOverlay';
            overlay.style.cssText = `
                position: fixed; top: 0; left: 0; width: 100%; height: 100%; 
                background: rgba(0,0,0,0.5); z-index: 9999; 
                display: flex; align-items: center; justify-content: center;
            `;
            overlay.innerHTML = `
                <div class="spinner-border text-light" role="status" style="width: 3rem; height: 3rem;">
                    <span class="visually-hidden">Caricamento...</span>
                </div>
            `;
            document.body.appendChild(overlay);
        }
    },

    hideLoading: function () {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.remove();
        }
    },

    showNotification: function (message, type = 'info') {
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 1050; max-width: 400px;';
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        document.body.appendChild(alertDiv);

        // Auto-rimozione dopo 5 secondi
        setTimeout(() => {
            if (alertDiv && alertDiv.parentNode) {
                alertDiv.remove();
            }
        }, 5000);
    }
};
