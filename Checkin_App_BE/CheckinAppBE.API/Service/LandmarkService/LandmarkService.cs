using Common;
using Dto.Landmark;
using Repository.Models;
using Repository.Repositories;
using Repository.UWO;

namespace Service.LandmarkService
{
    public class LandmarkService : ILandmarkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILandmarkRepository _landmarkRepository;

        public LandmarkService(IUnitOfWork unitOfWork, ILandmarkRepository landmarkRepository)
        {
            _unitOfWork = unitOfWork;
            _landmarkRepository = landmarkRepository;
        }

        public async Task<ServiceResult<IEnumerable<LandmarkResponseDto>>> GetAllLandmarksAsync()
        {
            var landmarks = await _landmarkRepository.GetAllAsync();
            var landmarkDtos = landmarks.Select(l => new LandmarkResponseDto
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                Address = l.Address,
                ImageUrl = l.ImageUrl
            });
            return ServiceResult<IEnumerable<LandmarkResponseDto>>.Success(landmarkDtos);
        }

        public async Task<ServiceResult<LandmarkResponseDto>> GetLandmarkByIdAsync(Guid id)
        {
            var landmark = await _landmarkRepository.GetByIdAsync(id);
            if (landmark == null)
            {
                return ServiceResult<LandmarkResponseDto>.Fail("Landmark not found.", 404);
            }
            var landmarkDto = new LandmarkResponseDto
            {
                Id = landmark.Id,
                Name = landmark.Name,
                Description = landmark.Description,
                Latitude = landmark.Latitude,
                Longitude = landmark.Longitude,
                Address = landmark.Address,
                ImageUrl = landmark.ImageUrl
            };
            return ServiceResult<LandmarkResponseDto>.Success(landmarkDto);
        }

        public async Task<ServiceResult<LandmarkResponseDto>> CreateLandmarkAsync(LandmarkCreateRequestDto request)
        {
            var landmark = new Landmark
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Address = request.Address,
                ImageUrl = request.ImageUrl
            };
            await _landmarkRepository.AddAsync(landmark);
            await _unitOfWork.CommitAsync();

            var landmarkDto = new LandmarkResponseDto
            {
                Id = landmark.Id,
                Name = landmark.Name,
                Description = landmark.Description,
                Latitude = landmark.Latitude,
                Longitude = landmark.Longitude,
                Address = landmark.Address,
                ImageUrl = landmark.ImageUrl
            };
            return ServiceResult<LandmarkResponseDto>.Success(landmarkDto);
        }

        public async Task<ServiceResult<LandmarkResponseDto>> UpdateLandmarkAsync(LandmarkUpdateRequestDto request)
        {
            var landmark = await _landmarkRepository.GetByIdAsync(request.Id);
            if (landmark == null)
            {
                return ServiceResult<LandmarkResponseDto>.Fail("Landmark not found.", 404);
            }

            landmark.Name = request.Name;
            landmark.Description = request.Description;
            landmark.Latitude = request.Latitude;
            landmark.Longitude = request.Longitude;
            landmark.Address = request.Address;
            landmark.ImageUrl = request.ImageUrl;

            _landmarkRepository.Update(landmark);
            await _unitOfWork.CommitAsync();

            var landmarkDto = new LandmarkResponseDto
            {
                Id = landmark.Id,
                Name = landmark.Name,
                Description = landmark.Description,
                Latitude = landmark.Latitude,
                Longitude = landmark.Longitude,
                Address = landmark.Address,
                ImageUrl = landmark.ImageUrl
            };
            return ServiceResult<LandmarkResponseDto>.Success(landmarkDto);
        }

        public async Task<ServiceResult<bool>> DeleteLandmarkAsync(Guid id)
        {
            var landmark = await _landmarkRepository.GetByIdAsync(id);
            if (landmark == null)
            {
                return ServiceResult<bool>.Fail("Landmark not found.", 404);
            }

            _landmarkRepository.Delete(landmark);
            await _unitOfWork.CommitAsync();
            return ServiceResult<bool>.Success(true);
        }
    }
}