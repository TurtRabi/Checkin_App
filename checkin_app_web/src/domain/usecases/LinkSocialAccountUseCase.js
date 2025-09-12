// src/domain/usecases/LinkSocialAccountUseCase.js

export default class LinkSocialAccountUseCase {
    constructor(authRepository) {
        this.authRepository = authRepository;
    }

    execute(provider, credential) {
        return this.authRepository.linkSocialAccount(provider, credential);
    }
}
