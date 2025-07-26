import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:app_map/presentation/viewmodels/collection_viewmodel.dart';
import 'package:app_map/core/di.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  late CollectionViewModel _collectionViewModel;

  @override
  void initState() {
    super.initState();
    _collectionViewModel = sl<CollectionViewModel>();
    _collectionViewModel.loadMemories(); // Load memories to get statistics
  }

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<CollectionViewModel>.value(
      value: _collectionViewModel,
      child: Consumer<CollectionViewModel>(
        builder: (context, viewModel, child) {
          return Scaffold(
            appBar: AppBar(
              title: const Text('Cá nhân'),
            ),
            body: SingleChildScrollView(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Hồ sơ cá nhân
                  Card(
                    margin: const EdgeInsets.only(bottom: 16.0),
                    child: Padding(
                      padding: const EdgeInsets.all(16.0),
                      child: Row(
                        children: [
                          const CircleAvatar(
                            radius: 40,
                            backgroundImage: NetworkImage('https://via.placeholder.com/150'), // Placeholder Avatar
                          ),
                          const SizedBox(width: 16),
                          Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              const Text(
                                'Tên: Anh Yêu',
                                style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                              ),
                              Text(
                                'Lv. ${12 + (viewModel.totalCards ~/ 10)}', // Cấp độ tăng theo số thẻ
                                style: const TextStyle(fontSize: 16, color: Colors.grey),
                              ),
                              Text(
                                '${viewModel.totalCards} thẻ đã sưu tầm',
                                style: const TextStyle(fontSize: 16, color: Colors.grey),
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ),

                  // Thống kê
                  _buildSectionTitle('Thống kê'),
                  Card(
                    margin: const EdgeInsets.only(bottom: 16.0),
                    child: Padding(
                      padding: const EdgeInsets.all(16.0),
                      child: Column(
                        children: [
                          _buildStatRow('Đã đến', '${viewModel.uniqueLocations} địa danh | ${viewModel.uniqueLocations ~/ 5} tỉnh'), // Giả định 5 địa danh/tỉnh
                          _buildStatRow('Thẻ bài', '${viewModel.totalCards} thẻ | ⭐: ${viewModel.rareCards}'),
                        ],
                      ),
                    ),
                  ),

                  // Hành trình của bạn
                  _buildSectionTitle('Hành trình của bạn'),
                  Card(
                    margin: const EdgeInsets.only(bottom: 16.0),
                    child: Padding(
                      padding: const EdgeInsets.all(16.0),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text('Danh sách hoặc bản đồ các điểm đã đến (TODO)'),
                          // TODO: Implement actual list/map of journeys
                        ],
                      ),
                    ),
                  ),

                  // Thành tựu
                  _buildSectionTitle('Thành tựu'),
                  Card(
                    margin: const EdgeInsets.only(bottom: 16.0),
                    child: Padding(
                      padding: const EdgeInsets.all(16.0),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          _buildAchievementRow(true, 'Check-in 10 địa điểm'),
                          _buildAchievementRow(false, 'Thẻ Sử Thi đầu tiên'),
                          // TODO: Add more achievements
                        ],
                      ),
                    ),
                  ),

                  // Mục tiêu cá nhân
                  _buildSectionTitle('Mục tiêu cá nhân (tuỳ chọn)'),
                  Card(
                    margin: const EdgeInsets.only(bottom: 16.0),
                    child: Padding(
                      padding: const EdgeInsets.all(16.0),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          _buildGoalRow(false, 'Đến 63 tỉnh'),
                          _buildGoalRow(true, 'Bộ sưu tập Văn hóa Sài Gòn'),
                          // TODO: Allow users to set custom goals
                        ],
                      ),
                    ),
                  ),

                  // Cài đặt
                  _buildSectionTitle('Cài đặt'),
                  Card(
                    margin: const EdgeInsets.only(bottom: 16.0),
                    child: Column(
                      children: const [
                        ListTile(
                          leading: Icon(Icons.language),
                          title: Text('Ngôn ngữ'),
                          subtitle: Text('Tiếng Việt'),
                        ),
                        ListTile(
                          leading: Icon(Icons.location_on),
                          title: Text('Bán kính tìm kiếm'),
                          subtitle: Text('20km'),
                        ),
                        ListTile(
                          leading: Icon(Icons.offline_bolt),
                          title: Text('Chế độ Offline'),
                          trailing: Switch(value: true, onChanged: null), // Placeholder
                        ),
                        ListTile(
                          leading: Icon(Icons.dark_mode),
                          title: Text('Dark Mode'),
                          trailing: Switch(value: false, onChanged: null), // Placeholder
                        ),
                      ],
                    ),
                  ),

                  // Đồng bộ tài khoản
                  _buildSectionTitle('Đồng bộ tài khoản'),
                  Card(
                    margin: const EdgeInsets.only(bottom: 16.0),
                    child: const ListTile(
                      leading: Icon(Icons.cloud_upload),
                      title: Text('Đã kết nối với Google'),
                      subtitle: Text('Lần cuối: hôm qua'),
                    ),
                  ),

                  // Bảo mật & Tài khoản
                  _buildSectionTitle('Bảo mật & Tài khoản'),
                  Card(
                    child: Column(
                      children: const [
                        ListTile(
                          leading: Icon(Icons.lock),
                          title: Text('Đổi mật khẩu'),
                        ),
                        ListTile(
                          leading: Icon(Icons.logout),
                          title: Text('Đăng xuất'),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildSectionTitle(String title) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8.0),
      child: Text(
        title,
        style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
      ),
    );
  }

  Widget _buildStatRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4.0),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(label, style: const TextStyle(fontSize: 16)),
          Text(value, style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
        ],
      ),
    );
  }

  Widget _buildAchievementRow(bool completed, String text) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4.0),
      child: Row(
        children: [
          Icon(completed ? Icons.check_circle : Icons.hourglass_empty,
              color: completed ? Colors.green : Colors.orange),
          const SizedBox(width: 8),
          Text(text, style: const TextStyle(fontSize: 16)),
        ],
      ),
    );
  }

  Widget _buildGoalRow(bool completed, String text) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4.0),
      child: Row(
        children: [
          Icon(completed ? Icons.check_box : Icons.check_box_outline_blank),
          const SizedBox(width: 8),
          Text(text, style: const TextStyle(fontSize: 16)),
        ],
      ),
    );
  }
}