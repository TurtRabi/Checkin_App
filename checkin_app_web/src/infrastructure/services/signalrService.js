import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useAuthStore } from '@/application/stores/auth';

const HUB_URL = "http://localhost:5027/notificationHub";

const connection = new HubConnectionBuilder()
  .withUrl(HUB_URL, {
    accessTokenFactory: () => {
      const authStore = useAuthStore();
      return authStore.accessToken;
    }
  })
  .configureLogging(LogLevel.Information)
  .withAutomaticReconnect()
  .build();

function setupSignalRListeners() {
  connection.on("ReceiveMessage", (user, message) => {
    console.log("SignalR: Nhận được 'ReceiveMessage'", { user, message });

    if (user && user.id) {
      const authStore = useAuthStore();
      if (authStore.user && authStore.user.id === user.id) {
        console.log("SignalR: Cập nhật thông tin người dùng cục bộ.");
        authStore.updateUserProfile(user);
      }
    }
  });
}

export async function startSignalRConnection() {
  if (connection.state === 'Disconnected') {
    try {
      await connection.start();
      console.log("Đã kết nối tới SignalR Hub thành công.");
      setupSignalRListeners();
    } catch (err) {
      console.error("Kết nối SignalR thất bại: ", err);
      setTimeout(startSignalRConnection, 5000);
    }
  }
}

export async function stopSignalRConnection() {
  if (connection.state === 'Connected') {
    try {
      await connection.stop();
      console.log("Đã ngắt kết nối SignalR Hub.");
    } catch (err) {
      console.error("Lỗi khi ngắt kết nối SignalR: ", err);
    }
  }
}

connection.onreconnecting(error => {
  console.warn(`Kết nối SignalR đang bị mất, đang thử kết nối lại... Lỗi: ${error}`);
});

connection.onreconnected(connectionId => {
  console.log(`Đã kết nối lại SignalR thành công với ID: ${connectionId}`);
});

connection.onclose(error => {
  console.error(`Kết nối SignalR đã đóng. Lỗi: ${error}`);
});