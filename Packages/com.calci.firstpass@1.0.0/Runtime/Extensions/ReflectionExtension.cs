using System;

public static class ReflectionExtension
{
	public static bool IsSubclassOfRawGeneric(this Type checkTargetType, Type baseType)
	{
		while (checkTargetType != null && checkTargetType != typeof(object))
		{
			Type cur = checkTargetType.IsGenericType ? checkTargetType.GetGenericTypeDefinition() : checkTargetType;
			if (baseType == cur)
			{
				return true;
			}

			checkTargetType = checkTargetType.BaseType;
		}

		return false;
	}
}