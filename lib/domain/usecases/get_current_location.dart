import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:app_map/domain/repositories/location_repository.dart';

class GetCurrentLocation {
  final LocationRepository repository;

  GetCurrentLocation(this.repository);

  Future<LatLng?> call() {
    return repository.getCurrentLocation();
  }
}
