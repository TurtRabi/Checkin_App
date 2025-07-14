import 'package:flutter/material.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Trang Chủ'),
      ),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // QuoteBanner
              Container(
                height: 100,
                color: Colors.blue.shade100,
                alignment: Alignment.center,
                child: const Text(
                  '"Cuộc sống là một hành trình, không phải điểm đến."',
                  textAlign: TextAlign.center,
                  style: TextStyle(fontSize: 18, fontStyle: FontStyle.italic),
                ),
              ),
              const SizedBox(height: 20),

              // DiscoverTodayCard
              Container(
                height: 150,
                color: Colors.green.shade100,
                alignment: Alignment.center,
                child: const Text(
                  'Khám phá hôm nay (Nút Khám phá hôm nay)',
                  textAlign: TextAlign.center,
                  style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                ),
              ),
              const SizedBox(height: 20),

              // SectionTitle "Địa danh nổi bật quanh bạn"
              const Text(
                'Địa danh nổi bật quanh bạn',
                style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 10),
              // HorizontalList(LocationCard)
              SizedBox(
                height: 150,
                child: ListView.builder(
                  scrollDirection: Axis.horizontal,
                  itemCount: 5,
                  itemBuilder: (context, index) {
                    return Card(
                      margin: const EdgeInsets.only(right: 10),
                      color: Colors.grey.shade200,
                      child: SizedBox(
                        width: 120,
                        child: Center(child: Text('Địa danh ${index + 1}')),
                      ),
                    );
                  },
                ),
              ),
              const SizedBox(height: 20),

              // SectionTitle "Khám phá theo chủ đề"
              const Text(
                'Khám phá theo chủ đề',
                style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 10),
              // ChipGroup
              Wrap(
                spacing: 8.0,
                children: ['Văn hóa', 'Thiên nhiên', 'Tĩnh lặng'].map((label) {
                  return Chip(label: Text(label));
                }).toList(),
              ),
              const SizedBox(height: 20),

              // SectionTitle "Hành trình nổi bật tuần này"
              const Text(
                'Hành trình nổi bật tuần này',
                style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 10),
              // JourneyCarousel(JourneyCard)
              SizedBox(
                height: 150,
                child: ListView.builder(
                  scrollDirection: Axis.horizontal,
                  itemCount: 3,
                  itemBuilder: (context, index) {
                    return Card(
                      margin: const EdgeInsets.only(right: 10),
                      color: Colors.purple.shade100,
                      child: SizedBox(
                        width: 200,
                        child: Center(child: Text('Hành trình ${index + 1}')),
                      ),
                    );
                  },
                ),
              ),
              const SizedBox(height: 20),

              // SectionTitle "Hành trình gần đây của bạn"
              const Text(
                'Hành trình gần đây của bạn',
                style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 10),
              // VerticalList(MyJourneyCard)
              Column(
                children: List.generate(2, (index) {
                  return Card(
                    margin: const EdgeInsets.only(bottom: 10),
                    color: Colors.orange.shade100,
                    child: ListTile(
                      title: Text('Hành trình của tôi ${index + 1}'),
                      subtitle: const Text('Mô tả ngắn gọn'),
                    ),
                  );
                }),
              ),
              const SizedBox(height: 20),

              // SectionTitle "Tiến độ sưu tầm thẻ bài"
              const Text(
                'Tiến độ sưu tầm thẻ bài',
                style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 10),
              // ProgressBar
              LinearProgressIndicator(
                value: 13 / 100,
                minHeight: 20,
                backgroundColor: Colors.grey.shade300,
                color: Colors.blue,
              ),
              const Padding(
                padding: EdgeInsets.only(top: 8.0),
                child: Text('13/100 thẻ', textAlign: TextAlign.center),
              ),
              const SizedBox(height: 20),

              // SectionTitle "Thử thách tuần này"
              const Text(
                'Thử thách tuần này',
                style: TextStyle(fontWeight:FontWeight.bold, fontSize: 20),
              ),
              const SizedBox(height: 10),
              // ChallengeCard
              Container(
                height: 100,
                color: Colors.red.shade100,
                alignment: Alignment.center,
                child: const Text(
                  'Thử thách: Tham quan 3 địa điểm lịch sử trong tuần',
                  textAlign: TextAlign.center,
                  style: TextStyle(fontSize: 16),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}