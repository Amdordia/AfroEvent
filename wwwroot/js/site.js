// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

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

		// also update dropdown items if present
		const dropdown = document.getElementById('notifItems');
		if (dropdown) {
			const item = document.createElement('div');
			item.className = 'small text-dark mb-2';
			item.textContent = message;
			dropdown.prepend(item);
		}

		// update badge count
		updateBadge(1);

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

function updateBadge(increment) {
	const badge = document.getElementById('notifBadge');
	if (!badge) return;
	let current = parseInt(badge.textContent || '0', 10);
	current = isNaN(current) ? 0 : current;
	current += increment;
	setBadge(current);
}
