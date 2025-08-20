export default class LoginWithGoogleUseCase {
  constructor(authRepository) {
    this.authRepository = authRepository;
  }

  async execute(googleToken) {
    if (!googleToken) {
      throw new Error("Google token is required");
    }
    return await this.authRepository.loginWithGoogle(googleToken);
  }
}
