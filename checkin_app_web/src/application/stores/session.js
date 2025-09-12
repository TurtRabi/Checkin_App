import { sessionUseCase } from "@/dependencies";
import { defineStore } from "pinia";
import { ref } from "vue";


export const useSessionStore = defineStore('session', () => {
    const sessions = ref([]);
    const isLoading = ref(false);
    const error = ref(null);
    const currentSessionId = ref(null);
    const fetchSessions = async () => {
        isLoading.value = true;
        error.value = null;
        try {
            const result = await sessionUseCase.getMySession();
            console.log("Fetched sessions:", result);
            sessions.value = result.sessions || [];
            currentSessionId.value = result.currentSessionId || null;
        } catch (err) {
            error.value = err.response?.data?.message || 'Lấy danh sách phiên thất bại.';
            sessions.value = [];
            currentSessionId.value = null;
        } finally {
            isLoading.value = false;
        }
    };

    const deleteSession = async (sessionId) => {
        if (!sessionId) {
            error.value = 'sessionId is required';
            return false;
        }
        isLoading.value = true;
        error.value = null;
        try {
            await sessionUseCase.deleteSession(sessionId);
            sessions.value = sessions.value.filter(s => s.id !== sessionId);
            return true;
        } catch (err) {
            error.value = err.response?.data?.message || 'Xóa phiên thất bại.';
            return false;
        }
        finally {
            isLoading.value = false;
        }   
    };
    const deleteAllSessionsExceptCurrent = async () => {
        isLoading.value = true;
        error.value = null;
        try {
            await sessionUseCase.deleteAllSessionsExceptCurrent();
            sessions.value = sessions.value.filter(s => s.id === currentSessionId.value);
            return true;
        } catch (err) {
            error.value = err.response?.data?.message || 'Xóa các phiên khác thất bại.';
            return false;
        }
        finally {
            isLoading.value = false;
        }
    };

    const fetchCurrentSession = async () => {
        isLoading.value = true;
        error.value = null;
        try {
            const result = await sessionUseCase.getCurrentSession();
            currentSessionId.value = result.id || null;
        } catch (err) {
            error.value = err.response?.data?.message || 'Lấy phiên hiện tại thất bại.';
            currentSessionId.value = null;
        } finally {
            isLoading.value = false;
        }       
    };

    // Initial fetch of sessions and current session
    fetchSessions();
    fetchCurrentSession();

    return {
        sessions,
        currentSessionId,
        isLoading,
        error,
        fetchSessions,
        deleteSession,
        deleteAllSessionsExceptCurrent,
        fetchCurrentSession
    };
});
        