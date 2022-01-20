# Rapid Code Blocks

## Fundamentals
네임스페이스를 임포트 할 필요가 없도록 모든 코드는 기본적으로 `global` 네임스페이스에서 작성되었습니다. 
필요시 네임스페이스를 지정할 수 있도록 `USE_CALCI_NAMESPACE` Scripting Define Symbol을 추가하여 네임스페이스를 사용하도록 지정할 수 있습니다.

## Includes
### Singletons
- [MonoInstance]() : 씬에서 직접 인스턴스를 생성해두어야 하는 싱글톤 컴포넌트
- [MonoSingleton]() : `Inst`에 최초 접근시 검색 후 인스턴스가 없는 경우 동적 생성되는 싱글톤 컴포넌트
- [PrefabSingleton]() : `Resources` 기능을 이용해 미리 지정된 프리팹을 Instantiate 하는 방식으로 동적 생성되는 싱글톤 컴포넌트
- [ScriptableObjectSingleton]() : `ScriptableObject` 오브젝트를 `Resources.Load`로 동적 로드되는 싱글톤 데이터 오브젝트
- [Singleton]() : `MonoBehaviour`를 상속받지 않은 네이티브 싱글톤 클래스
- [SingletonDebugger]() : `Singleton` 인스턴스 필드 확인용 디버거 클래스

### Components
- [DontDestroy]() : `DontDestroyOnLoad(this.gameObject)`를 호출 해 주는 컴포넌트
- [SceneViewOnly]() : `PlayMode`로 들어가면서 게임오브젝트 스스로 비활성화

### DataTypes
- [AnimationCurveAsset]() : `AnimationCurve` 타입을 저장하는 `ScriptableObject` 래핑 클래스
- [GradientAsset]() : `Gradient` 타입을 저장하는 `ScriptableObject` 래핑 클래스

### Extensions
- [CollectionExtension]() : 컬렉션 관련 확장 메서드 모음
- [MonoBehaviourExtension]() : `MonoBehaviour` 관련 확장 메서드 모음
- [ReflectionExtension]() : `System.Reflection` 관련 확장 메서드 모음
- [UGUIExtension]() : `UnityEngine.UI` 관련 확장 메서드 모음

### Utility
- [RichText]() : `string`을 `HTML` 컬러 RichText로 래핑해주는 유틸리티

[MonoInstance]: Runtime/Abstracts/MonoInstance.cs
[MonoSingleton]: Runtime/Abstracts/MonoSingleton.cs
[PrefabSingleton]: Runtime/Abstracts/PrefabSingleton.cs
[ScriptableObjectSingleton]: Runtime/Abstracts/ScriptableObjectSingleton.cs
[Singleton]: Runtime/Abstracts/Singleton.cs
[SingletonDebugger]: Runtime/Abstracts/SingletonDebugger.cs

[DontDestroy]: Runtime/Components/DontDestroy.cs
[SceneViewOnly]: Runtime/Components/SceneViewOnly.cs

[AnimationCurveAsset]: Runtime/DataTypes/AnimationCurveAsset.cs
[GradientAsset]: Runtime/DataTypes/GradientAsset.cs

[CollectionExtension]: Runtime/Extensions/CollectionExtension.cs
[MonoBehaviourExtension]: Runtime/Extensions/MonoBehaviourExtension.cs
[ReflectionExtension]: Runtime/Extensions/ReflectionExtension.cs
[UGUIExtension]: Runtime/Extensions/UGUIExtension.cs

[RichText]: Runtime/Utility/RichText.cs