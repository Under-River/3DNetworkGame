# 클레이 쉐이더 (Clay Shader)

이 쉐이더는 Unity URP에서 사용할 수 있는 클레이같은 질감의 커스텀 쉐이더입니다.

## 주요 특징

### 🏺 클레이 질감 시뮬레이션
- **서브서페이스 스캐터링**: 빛이 재질 내부로 들어가 산란되는 효과를 시뮬레이션하여 자연스러운 클레이 질감을 구현
- **Oren-Nayar 라이팅 모델**: 표준 람버트 라이팅보다 더 사실적인 거친 표면의 빛 반사를 구현
- **림 라이팅**: 가장자리에서 나타나는 부드러운 빛의 효과로 입체감 강화

### 🎨 다양한 프로퍼티 제어
- **베이스 컬러**: 클레이의 기본 색상 (기본값: 따뜻한 테라코타 색상)
- **서브서페이스 컬러**: 빛이 투과했을 때 나타나는 색상
- **Smoothness/Metallic**: 표면의 거칠기와 금속성 조절
- **Thickness Map**: 재질의 두께를 조절하여 서브서페이스 효과 강화

## 프로퍼티 설명

### Base Properties (기본 속성)
- **Base Color**: 클레이의 기본 색상
- **Base Map**: 베이스 텍스처 (선택사항)

### Surface Properties (표면 속성)
- **Smoothness**: 표면의 매끄러움 (0.0 = 매우 거침, 1.0 = 매우 매끄러움)
- **Metallic**: 금속성 (클레이의 경우 보통 0.0)
- **Normal Map**: 표면의 미세한 디테일을 위한 노멀 맵
- **Normal Scale**: 노멀 맵의 강도

### Clay Properties (클레이 특성)
- **Subsurface Color**: 서브서페이스 스캐터링 효과의 색상
- **Subsurface Strength**: 서브서페이스 효과의 강도 (0.0 ~ 2.0)
- **Thickness Map**: 재질의 두께를 나타내는 텍스처 (밝을수록 두꺼움)
- **Thickness Scale**: 두께 맵의 영향력 조절

### Rim Lighting (림 라이팅)
- **Rim Color**: 가장자리 빛의 색상
- **Rim Power**: 림 라이팅의 집중도 (낮을수록 넓게 퍼짐)
- **Rim Intensity**: 림 라이팅의 강도

### Ambient (주변광)
- **Ambient Occlusion**: 주변광 차폐 맵
- **AO Strength**: 주변광 차폐 효과의 강도

## 사용 방법

1. **머티리얼 생성**
   - 프로젝트 창에서 우클릭 → Create → Material
   - 새로 생성한 머티리얼의 Shader를 "Custom/ClayShader"로 변경

2. **기본 설정**
   - Base Color를 원하는 클레이 색상으로 설정 (예: 갈색, 주황색, 빨간색 등)
   - Smoothness를 0.2 ~ 0.4 사이로 설정 (클레이는 보통 매끄럽지 않음)
   - Metallic을 0.0으로 유지

3. **클레이 효과 강화**
   - Subsurface Color를 Base Color보다 밝고 따뜻한 색상으로 설정
   - Subsurface Strength를 0.5 ~ 1.2 사이로 조절
   - Rim Intensity를 0.3 ~ 0.8 사이로 설정하여 부드러운 가장자리 효과 추가

4. **세부 디테일 추가**
   - Normal Map을 사용하여 표면의 미세한 균열이나 질감 추가
   - Thickness Map을 사용하여 두께 변화가 있는 부분에서 더 강한 서브서페이스 효과 구현

## 권장 설정값

### 테라코타 클레이
- Base Color: (0.82, 0.54, 0.37, 1.0)
- Subsurface Color: (0.96, 0.71, 0.56, 1.0)
- Smoothness: 0.25
- Subsurface Strength: 0.8

### 적토 클레이
- Base Color: (0.65, 0.35, 0.25, 1.0)
- Subsurface Color: (0.85, 0.55, 0.45, 1.0)
- Smoothness: 0.2
- Subsurface Strength: 0.9

### 백토 클레이
- Base Color: (0.9, 0.85, 0.8, 1.0)
- Subsurface Color: (1.0, 0.95, 0.9, 1.0)
- Smoothness: 0.35
- Subsurface Strength: 0.6

## 성능 최적화

- 모바일 기기에서는 Subsurface Strength를 낮춰 성능을 향상시킬 수 있습니다
- 멀리 있는 오브젝트에는 Normal Map을 생략하여 성능을 개선할 수 있습니다
- LOD 시스템과 함께 사용하여 거리에 따라 셰이더 품질을 조절할 수 있습니다

## 호환성

- Unity 2021.3 LTS 이상
- Universal Render Pipeline (URP) 12.0 이상
- 모든 플랫폼 지원 (PC, Mobile, Console)

---

*이 쉐이더는 실시간 렌더링에 최적화되어 있으며, 다양한 클레이 재질을 표현할 수 있도록 설계되었습니다.* 