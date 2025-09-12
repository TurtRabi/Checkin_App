import ISessionRepository from "@/domain/repositories/ISessionRepository";
import apiClient from "@/infrastructure/api/apiClient";


export default class SessionRepository extends ISessionRepository {
    getMySession() {
        return apiClient.get(`/Session/me`);
    }  
    deleteSession(sessionId) {
        return apiClient.delete(`/Session/${sessionId}`);
    }
    deleteAllSessionsExceptCurrent() {
        return apiClient.delete(`/Session/me/all`);
    }
    getCurrentSession() {
        return apiClient.get(`/Session/me/current`);
    }
}