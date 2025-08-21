export default class LoginWithEmailPasswordUseCase {
    constructor(authRepository) {
        this.authRepository = authRepository;
    }

    async execute(username, password) {
        if(!username || !password) {
            throw new Error("Username and password are required");
        }
        return await this.authRepository.loginWithUsernameAndPassword(username, password);
    }
}