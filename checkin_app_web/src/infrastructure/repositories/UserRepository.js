import apiClient from "@/infrastructure/api/apiClient";
import IUserRepository from "@/domain/repositories/IUserRepository";
export default class UserRepository extends IUserRepository {
    async changePassword(email, username, newPassword) {
        return apiClient.post("User/forgot-password", { email: email, userName: username, newPassword: newPassword });
    }
}