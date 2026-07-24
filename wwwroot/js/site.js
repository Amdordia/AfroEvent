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

function renderNotifications() {
	const dropdown = document.getElementById('notifItems');
	if (!dropdown) return;

	const notifications = getStoredNotifications();
	dropdown.innerHTML = '';

	if (notifications.length === 0) {
		const empty = document.createElement('div');
		empty.className = 'text-muted small';
		empty.textContent = 'Aucune notification';
		dropdown.appendChild(empty);
	} else {
		// Display notifications, latest first
		notifications.forEach(notif => {
			const item = document.createElement('div');
			item.className = 'small text-dark mb-2 border-bottom pb-1';
			item.textContent = notif.message;
			if (!notif.read) {
				item.style.fontWeight = 'bold';
			}
			dropdown.appendChild(item);
		});
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
		read: false
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

		// show transient toast alert
		const container = document.createElement('div');
		container.style.position = 'fixed';
		container.style.top = '1rem';
		container.style.right = '1rem';
		container.style.zIndex = 1050;
		const alert = document.createElement('div');
		alert.className = 'alert alert-success shadow';
		alert.textContent = message;
		container.appendChild(alert);
		document.body.appendChild(container);
		setTimeout(() => document.body.removeChild(container), 5000);
	});

	connection.start().catch(err => console.error(err.toString()));
}

function toggleNotifications(e) {
	e.preventDefault();
	const dropdown = document.getElementById('notifDropdown');
	if (!dropdown) return;
	if (dropdown.style.display === 'none' || dropdown.style.display === '') {
		dropdown.style.display = 'block';
		markAllAsRead();
	} else {
		dropdown.style.display = 'none';
	}
}

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
		}
	} else {
		// Just render whatever we have stored
		renderNotifications();
	}
});
