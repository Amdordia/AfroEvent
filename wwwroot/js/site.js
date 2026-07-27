// AfroEvent Notifications & Utilities
const STORAGE_KEY = 'afroevent_notifications';

function getStoredNotifications() {
    try {
        return JSON.parse(localStorage.getItem(STORAGE_KEY)) || [];
    } catch (e) {
        return [];
    }
}

function saveNotifications(notifications) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(notifications));
}

function setBadge(count) {
    const badge = document.getElementById('notifBadge');
    if (!badge) return;
    if (count > 0) {
        badge.style.display = 'inline-block';
        badge.textContent = count > 99 ? '99+' : count;
    } else {
        badge.style.display = 'none';
    }
}

function renderNotifications() {
    const dropdown = document.getElementById('notifItems');
    if (!dropdown) return;

    const notifications = getStoredNotifications();
    dropdown.innerHTML = '';

    if (notifications.length === 0) {
        const empty = document.createElement('div');
        empty.className = 'text-muted small text-center py-4';
        empty.innerHTML = '<i class="bi bi-bell-slash fs-4 d-block mb-1 opacity-50"></i>Aucune notification';
        dropdown.appendChild(empty);
    } else {
        notifications.forEach(notif => {
            const item = document.createElement('div');
            item.className = 'px-3 py-2 border-bottom d-flex align-items-center justify-content-between gap-2 hover-bg-light';
            if (!notif.read) {
                item.style.backgroundColor = '#f0f7ff';
            }

            const contentDiv = document.createElement('div');
            contentDiv.className = 'flex-grow-1 overflow-hidden';

            const msgSpan = document.createElement('div');
            msgSpan.className = 'small text-dark text-break';
            msgSpan.style.fontSize = '0.85rem';
            msgSpan.textContent = notif.message;
            if (!notif.read) {
                msgSpan.style.fontWeight = '600';
            }
            contentDiv.appendChild(msgSpan);

            const btnGroup = document.createElement('div');
            btnGroup.className = 'd-flex align-items-center gap-1 flex-shrink-0';

            // QR Code button
            const qrBtn = document.createElement('button');
            qrBtn.type = 'button';
            qrBtn.className = 'btn btn-sm btn-outline-primary p-1 lh-1 rounded-circle';
            qrBtn.style.width = '28px';
            qrBtn.style.height = '28px';
            qrBtn.innerHTML = '<i class="bi bi-qr-code" style="font-size:0.85rem;"></i>';
            qrBtn.title = 'Afficher le QR code';
            qrBtn.onclick = (e) => {
                e.stopPropagation();
                showQRCode(notif.message);
            };

            // Delete button
            const delBtn = document.createElement('button');
            delBtn.type = 'button';
            delBtn.className = 'btn btn-sm btn-outline-danger p-1 lh-1 rounded-circle';
            delBtn.style.width = '28px';
            delBtn.style.height = '28px';
            delBtn.innerHTML = '<i class="bi bi-trash" style="font-size:0.85rem;"></i>';
            delBtn.title = 'Supprimer';
            delBtn.onclick = (e) => {
                e.stopPropagation();
                deleteNotification(notif.id);
            };

            btnGroup.appendChild(qrBtn);
            btnGroup.appendChild(delBtn);

            item.appendChild(contentDiv);
            item.appendChild(btnGroup);
            dropdown.appendChild(item);
        });
    }

    const unreadCount = notifications.filter(n => !n.read).length;
    setBadge(unreadCount);
}

function addNotification(message) {
    if (!message) return;
    const notifications = getStoredNotifications();
    notifications.unshift({
        id: Date.now() + '_' + Math.random().toString(36).substr(2, 6),
        message: message,
        read: false,
        date: new Date().toISOString()
    });
    if (notifications.length > 30) {
        notifications.pop();
    }
    saveNotifications(notifications);
    renderNotifications();
}

function deleteNotification(id) {
    let notifications = getStoredNotifications();
    notifications = notifications.filter(n => n.id !== id);
    saveNotifications(notifications);
    renderNotifications();
}

function markAllAsRead() {
    const notifications = getStoredNotifications();
    notifications.forEach(n => n.read = true);
    saveNotifications(notifications);
    setBadge(0);
    renderNotifications();
}

function toggleNotifications(e) {
    if (e) e.preventDefault();
    const dropdown = document.getElementById('notifDropdown');
    if (!dropdown) return;
    if (dropdown.style.display === 'none' || dropdown.style.display === '') {
        dropdown.style.display = 'block';
        markAllAsRead();
    } else {
        dropdown.style.display = 'none';
    }
}

// Close notification dropdown when clicking outside
document.addEventListener('click', function (e) {
    const dropdown = document.getElementById('notifDropdown');
    const toggle = document.getElementById('notifToggle');
    if (dropdown && dropdown.style.display === 'block') {
        if (!dropdown.contains(e.target) && !toggle?.contains(e.target)) {
            dropdown.style.display = 'none';
        }
    }
});

// Show QR Code in Modal
function showQRCode(message) {
    const modal = document.getElementById('qrModal');
    const canvas = document.getElementById('qrCanvas');
    const preview = document.getElementById('qrTextPreview');

    if (!modal || !canvas) {
        console.error('QR modal or canvas elements not found');
        return;
    }

    if (preview) {
        preview.textContent = message;
    }

    if (typeof QRCode === 'undefined') {
        console.error('QRCode library is not loaded');
        alert('Erreur: La bibliothèque QR Code est indisponible.');
        return;
    }

    // Render QR Code onto canvas
    QRCode.toCanvas(canvas, message, {
        width: 220,
        margin: 2,
        color: {
            dark: '#000000',
            light: '#FFFFFF'
        },
        errorCorrectionLevel: 'M'
    }, function (error) {
        if (error) {
            console.error('Erreur génération QR code:', error);
        }
    });

    modal.style.display = 'flex';
}

function closeQRModal() {
    const modal = document.getElementById('qrModal');
    if (modal) {
        modal.style.display = 'none';
    }
}

// SignalR Real-time Notifications
if (typeof signalR !== 'undefined') {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/eventHub')
        .withAutomaticReconnect()
        .build();

    connection.on('ReceiveNotification', function (message) {
        addNotification(message);

        // Toast notification
        const container = document.createElement('div');
        container.style.position = 'fixed';
        container.style.bottom = '1.5rem';
        container.style.right = '1.5rem';
        container.style.zIndex = 1050;
        
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-warning shadow-lg border-0 d-flex align-items-center gap-2 animate-fadein';
        alertDiv.style.borderRadius = '0.75rem';
        alertDiv.innerHTML = `<i class="bi bi-bell-fill text-dark"></i> <span class="fw-semibold small text-dark">${message}</span>`;
        
        container.appendChild(alertDiv);
        document.body.appendChild(container);
        setTimeout(() => {
            if (container.parentNode) {
                container.parentNode.removeChild(container);
            }
        }, 5000);
    });

    connection.start().catch(err => console.error('SignalR error:', err.toString()));
}

// Document Ready Initialization
document.addEventListener('DOMContentLoaded', () => {
    const helper = document.getElementById('sessionNotificationHelper');
    if (helper) {
        const message = helper.getAttribute('data-message');
        if (message) {
            addNotification(message);
        }
    }
    renderNotifications();
});
