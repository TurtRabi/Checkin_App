export default class registerWithUsernameAndPasswordUseCase {
    constructor(authRepository) {
        this.authRepository = authRepository;
    }

    async execute(username, password, displayName) {
        if (!username || !password || !displayName) {
            throw new Error("Username, password, and display name are required");
        }
        return await this.authRepository.registerWithUsernameAndPassword(username, password, displayName);
    }
}