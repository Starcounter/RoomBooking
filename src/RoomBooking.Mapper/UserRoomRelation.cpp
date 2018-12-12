template <>
struct mapper<RoomBooking::UserRoomRelation>
{
	static M::Entity on_create()
	{
		return HaveRelation.create();
	}

	static void on_update_User(entity root, const option<RoomBooking::User> &value)
	{
		root.subject(entity::from(*value));
	}

	static void on_update_Room(entity root, const option<RoomBooking::Room> &value)
	{
		root.object(entity::from(*value));
	}

	static void on_delete(entity root)
	{
		root.untrack_including_relations();
	}

	static void invalidate(change_type type, const entity &obj, const entity::changes &changes, entity::invset &inval)
	{
		const auto F = obj.features() + (changes.features_assigned() ? changes.features() : featureset{});

		if (HaveRelation.test(F))
		{
			inval(obj);
		}

		if (UserRelation.test(F))
		{
			from(obj).subject_of(HaveRelation).filter_object(Room).for_each(inval);
		}

		if (Room.test(F))
		{
			from(obj).object_of(HaveRelation).filter_subject(UserRelation).for_each(inval);
		}
	}

	static bool refresh(const entity &root, RoomBooking::UserRoomRelation::refresh_batch &changes)
	{
		if (!HaveRelation.test(root))
		{
			return false;
		}

		const auto user = root.subject();
		const auto room = root.object();

		if (user && user->exists() && !UserRelation.test(*user))
		{
			return false;
		}
		if (room && room->exists() && !Room.test(*room))
		{
			return false;
		}

		changes.User(user ? some(user->to<RoomBooking::User>()) : none);
		changes.Room(room ? some(room->to<RoomBooking::Room>()) : none);

		return true;
	}
};