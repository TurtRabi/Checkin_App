import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:app_map/domain/repositories/directions_repository.dart';

class GetDirections {
  final DirectionsRepository repository;

  GetDirections(this.repository);

  Future<List<LatLng>> call(LatLng origin, LatLng destination) {
    return repository.getDirections(origin, destination);
  }
}
