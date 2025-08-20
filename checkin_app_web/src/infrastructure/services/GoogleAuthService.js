import { googleTokenLogin } from "vue3-google-login"

export const useGoogleAuthService = () => {
  const login = () => {
    return new Promise((resolve, reject) => {
      googleTokenLogin().then(response => {
        if (response.access_token) {
          resolve(response.access_token);
        } else {
          reject("No access token found");
        }
      }).catch(error => {
        console.error("Google login error:", error);
        reject(error);
      });
    });
  };

  return { login };
};