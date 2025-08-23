export default class IUserRepository {
    changePassword(email, username, newPassword) {
        throw new Error("UserRepository must implement changePassword()");
    }
}