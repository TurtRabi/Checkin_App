export default class UserUseCase{
    constructor(userRepository) {
        this.userRepository = userRepository;
    }
    async changePassword(email, username, newPassword) {
        console.log(email);
        console.log(username);
        console.log(newPassword);
        if (!email || !username || !newPassword) {
            throw new Error("Email, username and newPassword are required");
        }
        return await this.userRepository.changePassword(email, username, newPassword);
    }
}