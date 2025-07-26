import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:app_map/domain/repositories/directions_repository.dart';
import 'package:app_map/data/datasources/directions_datasource.dart';

class DirectionsRepositoryImpl implements DirectionsRepository {
  final DirectionsDatasource remoteDatasource;

  DirectionsRepositoryImpl(this.remoteDatasource);

  @override
  Future<List<LatLng>> getDirections(LatLng origin, LatLng destination) {
    return remoteDatasource.getDirections(origin, destination);
  }
}
