export default class SessionUseCase {
    constructor(sessionRepository) {
        this.sessionRepository = sessionRepository;
    }
    async getMySession() {
        return await this.sessionRepository.getMySession();
    }
    async deleteSession(sessionId) {
        if (!sessionId) {
            throw new Error("sessionId is required");
        }   
        return await this.sessionRepository.deleteSession(sessionId);
    }
    async deleteAllSessionsExceptCurrent() {
        return await this.sessionRepository.deleteAllSessionsExceptCurrent();
    }
    async getCurrentSession() {
        return await this.sessionRepository.getCurrentSession();
    }
}