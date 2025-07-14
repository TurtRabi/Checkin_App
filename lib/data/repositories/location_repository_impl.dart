import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:app_map/domain/repositories/location_repository.dart';
import 'package:app_map/data/datasources/location_datasource.dart';

class LocationRepositoryImpl implements LocationRepository {
  final LocationDatasource remoteDatasource;

  LocationRepositoryImpl(this.remoteDatasource);

  @override
  Future<LatLng?> getCurrentLocation() {
    return remoteDatasource.getCurrentLocation();
  }

  @override
  Future<Map<String, dynamic>> generateRandomLocation(LatLng center, double radiusInKm) {
    return remoteDatasource.generateRandomLocation(center, radiusInKm);
  }
}
