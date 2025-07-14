import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:app_map/domain/repositories/location_repository.dart';

class GenerateRandomLocation {
  final LocationRepository repository;

  GenerateRandomLocation(this.repository);

  Future<Map<String, dynamic>> call(LatLng center, double radiusInKm) {
    return repository.generateRandomLocation(center, radiusInKm);
  }
}
