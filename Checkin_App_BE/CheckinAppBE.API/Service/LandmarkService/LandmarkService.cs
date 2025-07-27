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
                ImageUrl = l.ImageUrl,
                Status = l.Status
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
                ImageUrl = request.ImageUrl,
                Status = "Pending"
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

        public async Task<ServiceResult<IEnumerable<LandmarkResponseDto>>> GetPendingLandmarksAsync()
        {
            var pendingLandmarks = await _landmarkRepository.GetByConditionAsync(l => l.Status == "Pending");
            if (pendingLandmarks == null || !pendingLandmarks.Any())
            {
                return ServiceResult<IEnumerable<LandmarkResponseDto>>.Fail("No pending landmarks found.", 404);
            }
            var landmarkDtos = pendingLandmarks.Select(l => new LandmarkResponseDto
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                Address = l.Address,
                ImageUrl = l.ImageUrl,
                Status = l.Status
            });

            return ServiceResult<IEnumerable<LandmarkResponseDto>>.Success(landmarkDtos);
        }

        public async Task<ServiceResult> ApproveLandmarkAsync(Guid landmarkId)
        {
            var getPendingLandmarks = await _landmarkRepository.GetByIdAsync(landmarkId);
            if (getPendingLandmarks == null || getPendingLandmarks.Status != "Pending")
            {
                return ServiceResult.Fail("Landmark not found or not pending.", 404);
            }
            getPendingLandmarks.Status = "Approved";
            _landmarkRepository.Update(getPendingLandmarks);
            await _unitOfWork.CommitAsync();

            
            return ServiceResult.Success("sucessfull");
        }

        public async Task<ServiceResult> RejectLandmarkAsync(Guid landmarkId)
        {
            var getPendingLandmarks = await _landmarkRepository.GetByIdAsync(landmarkId);
            if (getPendingLandmarks == null || getPendingLandmarks.Status != "Pending")
            {
                return ServiceResult.Fail("Landmark not found or not pending.", 404);
            }
            getPendingLandmarks.Status = "Rejected";
            _landmarkRepository.Update(getPendingLandmarks);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("sucessfull");

        }
    }
}