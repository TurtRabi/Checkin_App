class Memory {
  final int? id;
  final double latitude;
  final double longitude;
  final String imagePath;
  final String description;
  final DateTime timestamp;
  final String safetyLevel;
  final String locationName;
  final String rarity; // New field

  Memory({
    this.id,
    required this.latitude,
    required this.longitude,
    required this.imagePath,
    required this.description,
    required this.timestamp,
    required this.safetyLevel,
    required this.locationName,
    required this.rarity, // New field
  });

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'latitude': latitude,
      'longitude': longitude,
      'imagePath': imagePath,
      'description': description,
      'timestamp': timestamp.toIso8601String(),
      'safetyLevel': safetyLevel,
      'locationName': locationName,
      'rarity': rarity, // New field
    };
  }

  static Memory fromMap(Map<String, dynamic> map) {
    return Memory(
      id: map['id'],
      latitude: map['latitude'],
      longitude: map['longitude'],
      imagePath: map['imagePath'],
      description: map['description'],
      timestamp: DateTime.parse(map['timestamp']),
      safetyLevel: map['safetyLevel'],
      locationName: map['locationName'],
      rarity: map['rarity'], // New field
    );
  }
}
