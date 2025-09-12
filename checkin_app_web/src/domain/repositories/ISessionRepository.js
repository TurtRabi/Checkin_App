export default class ISessionRepository {
    getMySession() {
        throw new Error("ISessionRepository must implement getMySession()");
    }

    deleteSession(sessionId) {
        throw new Error("ISessionRepository must implement deleteSession()");
    }

    deleteAllSessionsExceptCurrent() {
        throw new Error("ISessionRepository must implement deleteAllSessionsExceptCurrent()");
    }
    getCurrentSession() {
        throw new Error("ISessionRepository must implement getcurrentSession()");
    }
}