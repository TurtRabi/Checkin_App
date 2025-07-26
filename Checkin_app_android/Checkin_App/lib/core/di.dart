import 'package:get_it/get_it.dart';
import 'package:app_map/database/database_helper.dart';
import 'package:app_map/data/datasources/local_memory_datasource.dart';
import 'package:app_map/data/datasources/location_datasource.dart';
import 'package:app_map/data/datasources/directions_datasource.dart'; // New
import 'package:app_map/data/repositories/memory_repository_impl.dart';
import 'package:app_map/data/repositories/location_repository_impl.dart';
import 'package:app_map/data/repositories/directions_repository_impl.dart'; // New
import 'package:app_map/domain/repositories/memory_repository.dart';
import 'package:app_map/domain/repositories/location_repository.dart';
import 'package:app_map/domain/repositories/directions_repository.dart'; // New
import 'package:app_map/domain/usecases/get_memories.dart';
import 'package:app_map/domain/usecases/insert_memory.dart';
import 'package:app_map/domain/usecases/delete_memory.dart';
import 'package:app_map/domain/usecases/get_current_location.dart';
import 'package:app_map/domain/usecases/generate_random_location.dart';
import 'package:app_map/domain/usecases/get_directions.dart'; // New
import 'package:app_map/presentation/viewmodels/check_in_viewmodel.dart';
import 'package:app_map/presentation/viewmodels/collection_viewmodel.dart';
import 'package:app_map/presentation/viewmodels/map_viewmodel.dart';

final GetIt sl = GetIt.instance;

Future<void> init() async {
  // ViewModels
  sl.registerFactory(() => CheckInViewModel(sl(), sl(), sl(), sl(), sl()));
  sl.registerFactory(() => CollectionViewModel(sl(), sl()));
  sl.registerFactory(() => MapViewModel(sl(), sl(), sl()));

  // Use cases
  sl.registerLazySingleton(() => GetMemories(sl()));
  sl.registerLazySingleton(() => InsertMemory(sl()));
  sl.registerLazySingleton(() => DeleteMemory(sl()));
  sl.registerLazySingleton(() => GetCurrentLocation(sl()));
  sl.registerLazySingleton(() => GenerateRandomLocation(sl()));
  sl.registerLazySingleton(() => GetDirections(sl())); // New

  // Repositories
  sl.registerLazySingleton<MemoryRepository>(
    () => MemoryRepositoryImpl(sl()),
  );
  sl.registerLazySingleton<LocationRepository>(
    () => LocationRepositoryImpl(sl()),
  );
  sl.registerLazySingleton<DirectionsRepository>( // New
    () => DirectionsRepositoryImpl(sl()),
  );

  // Data sources
  sl.registerLazySingleton(() => LocalMemoryDatasource(sl()));
  sl.registerLazySingleton(() => LocationDatasource());
  sl.registerLazySingleton(() => DirectionsDatasource()); // New

  // External
  sl.registerLazySingleton(() => DatabaseHelper());
}