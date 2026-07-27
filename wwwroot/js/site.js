// Storage keys
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

// Format relative time (e.g., "il y a 2 min", "il y a 1h")
function formatRelativeTime(timestamp) {
	const now = Date.now();
	const diff = now - timestamp;
	const seconds = Math.floor(diff / 1000);
	const minutes = Math.floor(seconds / 60);
	const hours = Math.floor(minutes / 60);
	const days = Math.floor(hours / 24);

	if (seconds < 60) return "À l'instant";
	if (minutes < 60) return `Il y a ${minutes} min`;
	if (hours < 24) return `Il y a ${hours}h`;
	if (days === 1) return "Hier";
	return `Il y a ${days}j`;
}

// Detect notification type from message content
function getNotificationMeta(message) {
	const msg = message.toLowerCase();
	if (msg.includes('paiement') || msg.includes('payé') || msg.includes('billet')) {
		return { icon: 'bi-ticket-perforated-fill', color: '#28a745', bgColor: '#e8f5e9', label: 'Billetterie' };
	}
	if (msg.includes('annulé') || msg.includes('annulation') || msg.includes('erreur')) {
		return { icon: 'bi-exclamation-triangle-fill', color: '#dc3545', bgColor: '#fce4ec', label: 'Alerte' };
	}
	if (msg.includes('événement') || msg.includes('concert') || msg.includes('festival')) {
		return { icon: 'bi-calendar-event-fill', color: '#f77f00', bgColor: '#fff3e0', label: 'Événement' };
	}
	if (msg.includes('inscription') || msg.includes('bienvenue')) {
		return { icon: 'bi-person-check-fill', color: '#6f42c1', bgColor: '#f3e5f5', label: 'Inscription' };
	}
	return { icon: 'bi-bell-fill', color: '#ffb703', bgColor: '#fff8e1', label: 'Notification' };
}

function renderNotifications() {
	const dropdown = document.getElementById('notifItems');
	if (!dropdown) return;

	const notifications = getStoredNotifications();
	dropdown.innerHTML = '';

	if (notifications.length === 0) {
		const empty = document.createElement('div');
		empty.className = 'text-center py-4';
		empty.innerHTML = `
			<i class="bi bi-bell-slash text-muted" style="font-size: 2rem; display: block; margin-bottom: 0.5rem;"></i>
			<span class="text-muted small">Aucune notification pour le moment</span>
		`;
		dropdown.appendChild(empty);
	} else {
		// Display notifications, latest first
		notifications.forEach((notif, index) => {
			const meta = getNotificationMeta(notif.message);
			const item = document.createElement('div');
			item.className = 'notif-item d-flex align-items-start gap-2 p-2 rounded-3 mb-1';
			item.style.cssText = `
				transition: background-color 0.2s ease;
				cursor: pointer;
				border-left: 3px solid ${meta.color};
				background-color: ${notif.read ? 'transparent' : meta.bgColor};
			`;

			// Icon container
			const iconWrap = document.createElement('div');
			iconWrap.className = 'flex-shrink-0 d-flex align-items-center justify-content-center rounded-circle';
			iconWrap.style.cssText = `
				width: 32px; height: 32px; min-width: 32px;
				background-color: ${meta.color}15;
			`;
			iconWrap.innerHTML = `<i class="bi ${meta.icon}" style="color: ${meta.color}; font-size: 0.85rem;"></i>`;

			// Content container
			const content = document.createElement('div');
			content.className = 'flex-grow-1 overflow-hidden';

			const label = document.createElement('div');
			label.className = 'd-flex align-items-center justify-content-between mb-0';
			label.innerHTML = `
				<span class="fw-semibold" style="font-size: 0.7rem; color: ${meta.color}; text-transform: uppercase; letter-spacing: 0.5px;">${meta.label}</span>
				<span class="text-muted" style="font-size: 0.65rem;">${formatRelativeTime(notif.timestamp || notif.id)}</span>
			`;

			const msg = document.createElement('div');
			msg.className = 'small';
			msg.style.cssText = `
				color: #333;
				font-weight: ${notif.read ? '400' : '600'};
				line-height: 1.35;
				word-break: break-word;
			`;
			msg.textContent = notif.message;

			content.appendChild(label);
			content.appendChild(msg);

			item.appendChild(iconWrap);
			item.appendChild(content);

			// Hover effect
			item.addEventListener('mouseenter', () => {
				item.style.backgroundColor = '#f0f0f0';
			});
			item.addEventListener('mouseleave', () => {
				item.style.backgroundColor = notif.read ? 'transparent' : meta.bgColor;
			});

			dropdown.appendChild(item);
		});

		// Clear all button
		if (notifications.length > 0) {
			const clearBtn = document.createElement('div');
			clearBtn.className = 'text-center mt-2 pt-2 border-top';
			clearBtn.innerHTML = `
				<a href="#" class="small text-decoration-none text-danger" onclick="clearAllNotifications(event)">
					<i class="bi bi-trash3 me-1"></i>Tout effacer
				</a>
			`;
			dropdown.appendChild(clearBtn);
		}
	}

	// Update badge
	const unreadCount = notifications.filter(n => !n.read).length;
	setBadge(unreadCount);
}

function addNotification(message) {
	const notifications = getStoredNotifications();
	notifications.unshift({
		id: Date.now() + Math.random().toString(36).substr(2, 9),
		message: message,
		read: false,
		timestamp: Date.now()
	});
	// Keep a maximum of 20 notifications
	if (notifications.length > 20) {
		notifications.pop();
	}
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

function clearAllNotifications(e) {
	if (e) e.preventDefault();
	saveNotifications([]);
	renderNotifications();
}

// SignalR notification client
if (typeof signalR !== 'undefined') {
	const connection = new signalR.HubConnectionBuilder()
		.withUrl('/eventHub')
		.withAutomaticReconnect()
		.build();

	connection.on('ReceiveNotification', function (message) {
		// prepend to notifications list if present
		const list = document.getElementById('notificationsList');
		if (list) {
			const div = document.createElement('div');
			div.className = 'alert alert-light small mb-2';
			div.textContent = message;
			list.prepend(div);
		}

		// add notification to local storage and update dropdown
		addNotification(message);

		// show premium toast notification
		showToast(message);
	});

	connection.start().catch(err => console.error(err.toString()));
}

// Premium toast notification
function showToast(message) {
	const meta = getNotificationMeta(message);

	const container = document.createElement('div');
	container.style.cssText = `
		position: fixed;
		top: 1.5rem;
		right: 1.5rem;
		z-index: 9999;
		max-width: 380px;
		min-width: 300px;
		animation: slideInRight 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
	`;

	container.innerHTML = `
		<div style="
			background: #fff;
			border-radius: 12px;
			box-shadow: 0 8px 32px rgba(0,0,0,0.15), 0 2px 8px rgba(0,0,0,0.08);
			border-left: 4px solid ${meta.color};
			padding: 16px;
			display: flex;
			align-items: flex-start;
			gap: 12px;
			overflow: hidden;
			position: relative;
		">
			<div style="
				width: 40px; height: 40px; min-width: 40px;
				border-radius: 50%;
				background: ${meta.color}18;
				display: flex;
				align-items: center;
				justify-content: center;
			">
				<i class="bi ${meta.icon}" style="color: ${meta.color}; font-size: 1.1rem;"></i>
			</div>
			<div style="flex: 1; overflow: hidden;">
				<div style="font-weight: 700; font-size: 0.75rem; color: ${meta.color}; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 4px;">
					${meta.label}
				</div>
				<div style="font-size: 0.875rem; color: #333; line-height: 1.4; word-break: break-word;">
					${message}
				</div>
				<div style="font-size: 0.7rem; color: #999; margin-top: 6px;">
					À l'instant • AfroEvent
				</div>
			</div>
			<button onclick="this.closest('[style*=position]').remove()" style="
				background: none;
				border: none;
				color: #999;
				cursor: pointer;
				padding: 0;
				font-size: 1.1rem;
				line-height: 1;
				margin-top: -4px;
			">
				<i class="bi bi-x-lg"></i>
			</button>
			<div style="
				position: absolute;
				bottom: 0;
				left: 0;
				height: 3px;
				background: ${meta.color};
				animation: toastProgress 5s linear forwards;
			"></div>
		</div>
	`;

	document.body.appendChild(container);

	// Auto-remove after 5 seconds
	setTimeout(() => {
		if (container.parentElement) {
			container.style.animation = 'slideOutRight 0.3s ease forwards';
			setTimeout(() => {
				if (container.parentElement) {
					container.remove();
				}
			}, 300);
		}
	}, 5000);
}

// Inject animation keyframes
(function injectNotifStyles() {
	const style = document.createElement('style');
	style.textContent = `
		@keyframes slideInRight {
			from { transform: translateX(120%); opacity: 0; }
			to { transform: translateX(0); opacity: 1; }
		}
		@keyframes slideOutRight {
			from { transform: translateX(0); opacity: 1; }
			to { transform: translateX(120%); opacity: 0; }
		}
		@keyframes toastProgress {
			from { width: 100%; }
			to { width: 0%; }
		}
		#notifDropdown {
			border: none !important;
			border-radius: 12px !important;
			box-shadow: 0 8px 32px rgba(0,0,0,0.15), 0 2px 8px rgba(0,0,0,0.06) !important;
			max-height: 420px;
			overflow-y: auto;
		}
		#notifDropdown::-webkit-scrollbar {
			width: 4px;
		}
		#notifDropdown::-webkit-scrollbar-thumb {
			background: #ccc;
			border-radius: 2px;
		}
		.notif-item:hover {
			background-color: #f0f0f0 !important;
		}
	`;
	document.head.appendChild(style);
})();

function toggleNotifications(e) {
	e.preventDefault();
	e.stopPropagation();
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
	if (!dropdown || !toggle) return;

	// If the click is outside the dropdown and the toggle button
	if (!dropdown.contains(e.target) && !toggle.contains(e.target)) {
		dropdown.style.display = 'none';
	}
});

function setBadge(count) {
	const badge = document.getElementById('notifBadge');
	if (!badge) return;
	if (count > 0) {
		badge.style.display = 'inline-block';
		badge.textContent = count;
	} else {
		badge.style.display = 'none';
	}
}

// On document load
document.addEventListener('DOMContentLoaded', () => {
	// Check if there is a session notification from the server
	const helper = document.getElementById('sessionNotificationHelper');
	if (helper) {
		const message = helper.getAttribute('data-message');
		if (message) {
			addNotification(message);
			// Also show toast for session notifications
			showToast(message);
		}
	} else {
		// Just render whatever we have stored
		renderNotifications();
	}
});
